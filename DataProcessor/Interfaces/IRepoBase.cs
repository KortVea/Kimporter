using DataProcessor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataProcessor.Interfaces
{
    public interface IRepoBase<T> where T : EntityBase
    {
        string ConnStr { get; set; }
        Task<int> InsertAllAsync(IEnumerable<T> list, string connStr = null);
        Task<IEnumerable<T>> GetAllAsync(string connStr = null);

    }
}
