using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using DataProcessor.Models;

namespace DataProcessor.DAL
{
    public class TraceRepo : RepoBase<DownloadedTraceData>
    {
        public TraceRepo(string connStr) : base(connStr)
        {
        }

        public async Task<IEnumerable<DownloadedTraceData>> GetFullTraces()
        {
            string sql = $@"SELECT * FROM {nameof(DownloadedTraceData)} AS A 
                         LEFT JOIN {nameof(DownloadedPropertyData)} AS B 
                         ON B.TraceDataId = A.Id";
            using (var conn = new SqlConnection(_connStr))
            {
                var tracePropDic = new Dictionary<Guid, DownloadedTraceData>();

                var traces = await conn.QueryAsync<DownloadedTraceData, DownloadedPropertyData, DownloadedTraceData>(
                    sql,
                    (trace, property) =>
                    {
                        DownloadedTraceData traceEntry;
                        if (!tracePropDic.TryGetValue(trace.Id, out traceEntry))
                        {
                            traceEntry = trace;
                            traceEntry.DownloadedPropertyData = new List<DownloadedPropertyData>();
                            tracePropDic.Add(traceEntry.Id, traceEntry);
                        }
                        traceEntry.DownloadedPropertyData.Add(property);
                        return traceEntry;
                    }).ContinueWith(i => i.Result.Distinct().ToList());

                return traces;
            }
        }

        public async Task InsertTracesAndPropsWhileIgnoringSameHash(IEnumerable<DownloadedTraceData> list, IProgress<DbProgressInfo> progress = null,
                                                                    bool eagerLoadingHash = true, DateTime? end = null)
        {
            if (eagerLoadingHash)
            {
                await InsertWithQueryAllFirst(list, progress, end);
            }
            else
            {
                await InsertWithQueryOneByOne(list, progress);
            }
        }

        private async Task InsertWithQueryOneByOne(IEnumerable<DownloadedTraceData> list, IProgress<DbProgressInfo> progress)
        {
            var sqlHashExists = $@"SELECT COUNT(1) FROM {nameof(DownloadedTraceData)} WHERE Hash = @hash";
            using (var conn = new SqlConnection(_connStr))
            {
                await conn.OpenAsync();
                {
                    var tempCount = 0;
                    var totalCount = list.Count();
                    foreach (var item in list)
                    {
                        if (progress != null)
                        {
                            tempCount++;
                            progress.Report(new DbProgressInfo
                            {
                                ProgressType = ProgressType.Writing,
                                Number1 = tempCount,
                                Number2 = totalCount
                            });
                        }
                        //check each item's Hash against DB
                        var existing = await conn.ExecuteScalarAsync<bool>(sqlHashExists, new { Hash = item.Hash });
                        if (existing)
                        {
                            continue;
                        }
                        else
                        //write this item to DB
                        {
                            using (var trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    await conn.InsertAsync(item, trans);
                                    await conn.InsertAsync(item.DownloadedPropertyData, trans);
                                    trans.Commit();
                                }
                                catch (Exception)
                                {
                                    trans.Rollback();
                                    progress?.Report(new DbProgressInfo
                                    {
                                        ProgressType = ProgressType.Default,
                                        Message = $"Error writing {item.ToString()}"
                                    });
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task InsertWithQueryAllFirst(IEnumerable<DownloadedTraceData> list, IProgress<DbProgressInfo> progress, DateTime? end)
        {
            var sqlHashCount = $@"SELECT COUNT(*) FROM {nameof(DownloadedTraceData)}";
            var sqlHashPage1 = $@"SELECT [Hash] FROM {nameof(DownloadedTraceData)} ";
            var sqlHashPage2 = " ORDER BY [Hash] OFFSET @offset ROWS FETCH NEXT @batchSize ROWS ONLY";
            var sqlDatePredicate = string.Empty;
            if (end.HasValue)
            {
                sqlDatePredicate = $@" WHERE [Time] <= '{end.Value.ToString("yyyy-MM-dd hh:mm:ss.fff")}'";
                sqlHashCount += sqlDatePredicate;
            }
            var sqlHashPage = sqlHashPage1 + sqlDatePredicate + sqlHashPage2;

            using (var conn = new SqlConnection(_connStr))
            {
                await conn.OpenAsync();
                
                //counting hash
                progress?.Report(new DbProgressInfo
                {
                    ProgressType = ProgressType.Default,
                    Message = string.IsNullOrEmpty(sqlDatePredicate) ? "Couting Hash" : $"Counting Hash with\n{sqlDatePredicate}"
                });
                var hashCount = await conn.ExecuteScalarAsync<int>(sqlHashCount);

                //reading all hash pages
                var hashList = new List<long>();
                var batchSize = 1000;
                for (int i = 0; i <= hashCount / batchSize; i++)
                {
                    var offset = i * batchSize;
                    progress?.Report(new DbProgressInfo
                    {
                        ProgressType = ProgressType.Reading,
                        Number1 = offset,
                        Number2 = hashCount
                    });
                    var pagedHash = await conn.QueryAsync<long>(sqlHashPage, new { offset = offset, batchSize = batchSize });
                    hashList.AddRange(pagedHash);
                }

                //writing list of non-colliding hash to DB.
                var tempCount = 0;
                var listOfDifferentHashes = list.Where(i => !hashList.Contains(i.Hash));
                var totalCount = listOfDifferentHashes.Count();
                foreach (var item in listOfDifferentHashes)
                {
                    if (!hashList.Contains(item.Hash))
                    {
                        tempCount++;
                        progress?.Report(new DbProgressInfo
                        {
                            ProgressType = ProgressType.Writing,
                            Number1 = tempCount,
                            Number2 = totalCount
                        });
                        using (var trans = conn.BeginTransaction())
                        {
                            try
                            {
                                await conn.InsertAsync(item, trans);
                                await conn.InsertAsync(item.DownloadedPropertyData, trans);
                                trans.Commit();
                            }
                            catch (SqlException es) when (es.Number == 2627)
                            {
                                trans.Rollback();
                                progress?.Report(new DbProgressInfo
                                {
                                    ProgressType = ProgressType.Exception,
                                });
                                //Violation of Unique key constrain on Hash - Ignored
                            }
                            catch (Exception)
                            {
                                trans.Rollback();
                                progress?.Report(new DbProgressInfo
                                {
                                    ProgressType = ProgressType.Default,
                                    Message = $"Error writing {item.ToString()}"
                                });
                                throw;
                            }
                        }
                    }
                }
                
            }
        }
    }
}
