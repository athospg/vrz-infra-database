using System.Collections.Generic;
using System.Text;

namespace VRZ.Infra.Database.Abstractions.Set
{
    public interface IDatabaseSetBuilders<in TEntity>
    {
        void AddValueClause(StringBuilder builder, TEntity entity);

        void AddValueClause(StringBuilder builder, IEnumerable<TEntity> entities);

        void AddValueSeparator(StringBuilder builder);

        void AddMultiValueFinish(StringBuilder builder);
    }
}
