using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace VRZ.Infra.Database.Abstractions.Context
{
    public interface IDatabaseOperations
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = new());

        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = new());

        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = new());
    }
}
