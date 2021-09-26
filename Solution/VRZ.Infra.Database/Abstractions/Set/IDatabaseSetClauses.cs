using System.Collections.Generic;
using System.Text;

namespace VRZ.Infra.Database.Abstractions.Set
{
    public interface IDatabaseSetClauses<in TEntity>
    {
        string InsertColumns();

        void InsertColumns(StringBuilder query);

        string InsertClause();

        string ValueClause(TEntity entity);

        string ValueClause(IEnumerable<TEntity> entities);

        string InsertValueClause(TEntity entity);

        string InsertValueClause(IEnumerable<TEntity> entities);
    }
}
