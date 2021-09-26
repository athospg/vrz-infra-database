using System.Text;
using VRZ.Infra.Database.Abstractions.Context;
using VRZ.Infra.Database.Abstractions.Set;

namespace VRZ.Infra.Database.Postgresql
{
    public class NpgsqlSet<TEntity> : DatabaseSet<TEntity>, IDatabaseSet<TEntity>
    {
        public NpgsqlSet(IDatabaseContext context, string entityTypeName) : base(context, entityTypeName)
        {
        }


        public override string InsertClause()
        {
            var query = new StringBuilder($"INSERT INTO {Table} (");
            InsertColumns(query);

            query.Remove(query.Length - 1, 1).Append(") VALUES ");

            return query.ToString();
        }


        public override string ValueClause(TEntity entity)
        {
            var insertQuery = new StringBuilder("(");
            foreach (var prop in MappedPropertiesWithoutId)
            {
                insertQuery.Append($"{Converter(prop.GetValue(entity))},");
            }

            insertQuery.Remove(insertQuery.Length - 1, 1).Append(")");

            return insertQuery.ToString();
        }

        public override string InsertValueClause(TEntity entity)
        {
            return $"{InsertClause()}{ValueClause(entity)};";
        }

        public override void AddValueClause(StringBuilder builder, TEntity entity)
        {
            builder.Append("(");
            foreach (var prop in MappedPropertiesWithoutId)
            {
                builder.Append($"{Converter(prop.GetValue(entity))},");
            }

            builder.Remove(builder.Length - 1, 1).Append(")");
        }

        public override void AddValueSeparator(StringBuilder builder)
        {
            builder.Append(",");
        }

        public override void AddMultiValueFinish(StringBuilder builder)
        {
            builder.Remove(builder.Length - 1, 1).Append(";");
        }
    }
}
