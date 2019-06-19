using DataProcessor.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DataProcessor.Interfaces
{
    public interface IKMLParser
    {
        Task<IEnumerable<DownloadedTraceData>> Parse(Stream stream);
    }
}