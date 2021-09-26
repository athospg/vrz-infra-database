using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using VRZ.Infra.Database.Abstractions.Context;
using VRZ.Infra.Database.Utils;

namespace VRZ.Infra.Database.Abstractions.Set
{
    public abstract class DatabaseSet<TEntity> : IDatabaseSet<TEntity>
    {
        #region Attributes

        protected readonly IDatabaseContext Context;

        protected readonly string EntityTypeName;

        protected readonly List<PropertyInfo> MappedProperties;

        #endregion


        public DatabaseSet(IDatabaseContext context, string entityTypeName)
        {
            Context = context;
            EntityTypeName = entityTypeName;

            MappedProperties = typeof(TEntity).GetProperties().GetMappedProperties().ToList();
        }


        protected string Table => Context.GetTableName(typeof(TEntity));


        #region IDatabaseSet

        public Func<object, string> Converter => Context.Converter;


        public virtual string GetTable()
        {
            return Table;
        }

        #endregion

        #region IDatabaseSetClauses

        public virtual string InsertColumns()
        {
            var query = new StringBuilder();
            InsertColumns(query);
            return query.ToString();
        }

        public virtual void InsertColumns(StringBuilder query)
        {
            foreach (var prop in MappedProperties.Select(x => x.Name))
            {
                query.Append($"{prop},");
            }
        }

        public abstract string InsertClause();

        public abstract string ValueClause(TEntity entity);

        public virtual string ValueClause(IEnumerable<TEntity> entities)
        {
            var builder = new StringBuilder();
            AddValueClause(builder, entities);
            return builder.ToString();
        }

        public abstract string InsertValueClause(TEntity entity);

        public virtual string InsertValueClause(IEnumerable<TEntity> entities)
        {
            var builder = new StringBuilder(InsertClause());
            AddValueClause(builder, entities);
            AddMultiValueFinish(builder);
            return builder.ToString();
        }

        #endregion

        #region IDatabaseSetBuilders

        public abstract void AddValueClause(StringBuilder builder, TEntity entity);

        public virtual void AddValueClause(StringBuilder builder, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                AddValueClause(builder, entity);
                AddValueSeparator(builder);
            }
        }

        public abstract void AddValueSeparator(StringBuilder builder);

        public abstract void AddMultiValueFinish(StringBuilder builder);

        #endregion

        #region Bulk

        public virtual async Task<int> BulkInsert(IReadOnlyCollection<TEntity> entities, int batchSize,
            bool showLogs)
        {
            var affectedCount = 0;

            var sqlBatches = GetSqlBatches(entities, batchSize);
            using var connection = Context.Connection;

            foreach (var sql in sqlBatches)
            {
                try
                {
                    affectedCount += await connection.ExecuteAsync(sql);
                }
                catch (Exception)
                {
                    if (showLogs)
                    {
                        Console.WriteLine(sql);
                    }

                    throw;
                }
            }

            return affectedCount;
        }

        public IEnumerable<string> GetSqlBatches(IReadOnlyCollection<TEntity> entities, int batchSize)
        {
            var baseQuery = GetBulkInsertBaseQuery();
            var numberOfBatches = (int)Math.Ceiling((double)entities.Count / batchSize);

            for (var i = 0; i < numberOfBatches; i++)
            {
                var valuesClauses = ValueClause(entities
                    .Skip(i * batchSize)
                    .Take(batchSize));

                yield return string.Format(baseQuery, string.Join(",", valuesClauses));
            }
        }

        public virtual string GetBulkInsertBaseQuery()
        {
            return $@"with entities as (
    select *
    from (values {{0}}
         ) as t({InsertColumns()})
)
insert
into {Table} ({InsertColumns()})
select *
from entities
where not exists(
        select *
        from {Table}
        where {InsertColumns()}
    )
;";
        }

        #endregion
    }
}
