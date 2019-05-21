using System;
using System.Collections.Generic;
using System.Text;
using DataProcessor.Models;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataProcessor
{
    public class Repo
    {
        public static IEnumerable<DownloadedTraceData> GetAllTraces()
        {
            var query = @"SELECT * FROM DownloadedTraceData";
            using (var conn = new SqlConnection("Server=localhost;Database=Tester;Trusted_Connection=True;"))
            //using (var conn = new SqlConnection(@"Data Source=PC0011\SQLEXPRESS;Initial Catalog=Tester;Integrated Security=SSPI"))
            //using (var conn = new SqlConnection(@"Data Source=127.0.0.1,1433;Initial Catalog=Tester;User Id = yishi_liu@buildingpoint.com.au; Password=Lewis0017"))
            {
                var traces = conn.Query<DownloadedTraceData>(query);
                return traces;
            }
        }

        public static async Task<IEnumerable<DownloadedTraceData>> GetFullTrace()
        {
            string sql = @"SELECT * FROM [DownloadedTraceData] AS A " +
                         @"LEFT JOIN DownloadedPropertyData AS B " +
                         @"ON B.TraceDataId = A.Id";
            using (var conn = new SqlConnection("Server=localhost;Database=Tester;Trusted_Connection=True;"))
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
                    //.Distinct()
                    //.ToList();

                return traces;
            }
        }

    }
}
