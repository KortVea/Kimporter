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
    public class TraceRepo: RepoBase<DownloadedTraceData>
    {
        public TraceRepo(string connStr): base(connStr)
        {
        }
        public async Task<IEnumerable<DownloadedTraceData>> GetFullTraces()
        {
            string sql = @"SELECT * FROM DownloadedTraceData AS A " +
                         @"LEFT JOIN DownloadedPropertyData AS B " +
                         @"ON B.TraceDataId = A.Id";
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

        public async Task InsertTracesAndPropsWhileIgnoringSameHash(IEnumerable<DownloadedTraceData> list, IProgress<int> progress = null)
        {
            var sqlHashExists = $@"SELECT COUNT(1) FROM {nameof(DownloadedTraceData)} WHERE Hash = @hash";
            var sqlInsertIntoTrace = $@"INSERT INTO {nameof(DownloadedTraceData) }";
            var sqlInsertIntoProp = $@"";
            using (var conn = new SqlConnection(_connStr))
            {
                await conn.OpenAsync();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var tempCount = 0;
                        foreach (var item in list)
                        {
                            if (progress != null)
                            {
                                tempCount++;
                                progress.Report(tempCount);
                            }
                            var existing = await conn.ExecuteScalarAsync<bool>(sqlHashExists, new { Hash = item.Hash }, transaction: trans);
                            if (existing)
                            {
                                continue;
                            }
                            else
                            {
                                await conn.InsertAsync(item, trans);
                                await conn.InsertAsync(item.DownloadedPropertyData, trans);
                            }
                            
                        }
                        trans.Commit();
                    }
                    catch (Exception et)
                    {
                        trans.Rollback();
                        throw;
                    }
                    
                }

            }
        }

    }
}
