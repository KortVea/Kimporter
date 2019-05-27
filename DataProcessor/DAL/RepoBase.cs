using Dapper.Contrib.Extensions;
using DataProcessor.Models;
using DataProcessor.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataProcessor.DAL
{
    public class RepoBase<T>: IRepoBase<T> where T : EntityBase
    {
        public string ConnStr { get; set; } = String.Empty;

        public async Task<int> InsertAllAsync(IEnumerable<T> list, string connStr = null)
        {
            ConnStr = connStr == null ? ConnStr : connStr;
            using (var conn = new SqlConnection(ConnStr))
            {
                await conn.OpenAsync();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var affectedRows = await conn.InsertAsync(list);
                        trans.Commit();
                        return affectedRows;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(string connStr = null)
        {
            ConnStr = connStr == null ? ConnStr : connStr;
            using (var conn = new SqlConnection(ConnStr))
            {
                var traces = await conn.GetAllAsync<T>();
                return traces;
            }
        }

    }
}
