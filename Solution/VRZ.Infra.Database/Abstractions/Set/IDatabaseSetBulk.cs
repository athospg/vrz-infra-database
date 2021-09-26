using System.Collections.Generic;
using System.Threading.Tasks;

namespace VRZ.Infra.Database.Abstractions.Set
{
    public interface IDatabaseSetBulk<in TEntity>
    {
        Task<int> BulkInsert(IReadOnlyCollection<TEntity> entities, int batchSize = 500,
            bool showLogs = false);

        IEnumerable<string> GetSqlBatches(IReadOnlyCollection<TEntity> entities, int batchSize = 500);

        string GetBulkInsertBaseQuery();
    }
}
