using DataProcessor.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DataProcessor.Interfaces
{
    public interface IKMLParosr
    {
        Task<IEnumerable<DownloadedTraceData>> Parse(Stream stream);
    }
}