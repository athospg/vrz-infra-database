using System;
using System.Data;
using Npgsql;
using VRZ.Infra.Database.Abstractions.Context;
using VRZ.Infra.Database.Abstractions.Set;

namespace VRZ.Infra.Database.Postgresql
{
    public class NpgsqlContext : DatabaseContext, IDatabaseContext
    {
        public NpgsqlContext(string connectionString, string defaultSchema = null)
            : base(new NpgsqlSetSource(), connectionString, defaultSchema)
        {
        }

        public NpgsqlContext(IDatabaseSetSource databaseSetSource, string connectionString, string defaultSchema = null)
            : base(databaseSetSource, connectionString, defaultSchema)
        {
        }


        public override Func<object, string> Converter { get; } = NpgsqlExtensions.ToNpgsqlString;


        protected override IDbConnection GetNewConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}
