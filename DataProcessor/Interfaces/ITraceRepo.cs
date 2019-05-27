using DataProcessor.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataProcessor.Interfaces
{
    public interface ITraceRepo<T> : IRepoBase<T> where T : EntityBase
    {
        Task<IEnumerable<DownloadedTraceData>> GetFullTraces(string connStr = null);
        Task InsertTracesAndPropsWhileIgnoringSameHash(IEnumerable<DownloadedTraceData> list, IProgress<DbProgressInfo> progress = null,
                                                                    bool eagerLoadingHash = true, DateTime? end = null, string connStr = null);
        Task<IEnumerable<DownloadedTraceData>> InsertWithInMemoryCheck(IEnumerable<DownloadedTraceData> list, IProgress<DbProgressInfo> progress = null,
                                                                    DateTime? end = null, string connStr = null);

    }
}
