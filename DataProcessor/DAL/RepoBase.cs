using Dapper.Contrib.Extensions;
using DataProcessor.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataProcessor.DAL
{
    public class RepoBase<T> where T : EntityBase
    {
        protected string _connStr;
        public RepoBase(string connStr)
        {
            _connStr = connStr;
        }

        public async Task<int> InsertAllAsync(IEnumerable<T> list)
        {
            using (var conn = new SqlConnection(_connStr))
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

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var conn = new SqlConnection(_connStr))
            {
                var traces = await conn.GetAllAsync<T>();
                return traces;
            }
        }

    }
}
