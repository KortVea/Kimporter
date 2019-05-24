﻿using System;
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
                                                                    bool eagerLoadingHash = true, DateTime? start = null, DateTime? end = null)
        {
            if (eagerLoadingHash)
            {
                await InsertWithQueryAllFirst(list, progress, start, end);
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
                        //check Hash against DB
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

        private async Task InsertWithQueryAllFirst(IEnumerable<DownloadedTraceData> list, IProgress<DbProgressInfo> progress, DateTime? start, DateTime? end)
        {
            var sqlHashRange = $@"SELECT [Hash] FROM {nameof(DownloadedTraceData)}";
            var sqlHashCount = $@"SELECT COUNT(*) FROM {nameof(DownloadedTraceData)}";
            var sqlHashPage = $@"SELECT [Hash] FROM {nameof(DownloadedTraceData)} ORDER BY [Hash] OFFSET @offset ROWS FETCH NEXT @batchSize ROWS ONLY";
            var sqlDatePredicate = string.Empty;

            if (start.HasValue && end.HasValue)
            {
                sqlDatePredicate = $@" WHERE [Time] < {end.Value.ToString("yyyy-MM-dd")} AND [Time] >= {start.Value.ToString("yyyy-MM-dd")}";
                sqlHashRange += sqlDatePredicate;
                sqlHashCount += sqlDatePredicate;
            }

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
                for (int i = 0; i < hashCount / batchSize; i++)
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

                //writing hash to DB with local Hash list.
                var tempCount = 0;
                var totalCount = list.Count();
                foreach (var item in list)
                {
                    tempCount++;
                    progress?.Report(new DbProgressInfo
                    {
                        ProgressType = ProgressType.Writing,
                        Number1 = tempCount,
                        Number2 = totalCount
                    });
                    if (hashList.Contains(item.Hash))
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
}
