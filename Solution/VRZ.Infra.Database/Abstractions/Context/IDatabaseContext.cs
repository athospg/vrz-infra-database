using System;
using System.Data;
using VRZ.Infra.Database.Abstractions.Set;

namespace VRZ.Infra.Database.Abstractions.Context
{
    public interface IDatabaseContext : IDatabaseOperations
    {
        /// <summary>
        ///     Object to string default converter.
        /// </summary>
        Func<object, string> Converter { get; }

        IDbConnection Connection { get; }

        /// <summary>
        ///     Creates a <see cref="DatabaseSet{TEntity}" /> that can be used to query and save instances
        ///     of <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity"> The type of entity for which a set should be returned. </typeparam>
        /// <returns> A set for the given entity type. </returns>
        IDatabaseSet<TEntity> Set<TEntity>();

        /// <summary>
        ///     Creates a <see cref="IDatabaseSet{TEntity}" /> that can be used to query and save instances
        ///     of <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity"> The type of entity for which a set should be returned. </typeparam>
        /// <returns> A set for the given entity type. </returns>
        IDatabaseSet<TEntity> Set<TEntity>(string name);


        void SetEntityTable<TEntity>(string tableName, string schema = null);

        void SetEntityTable(Type type, string tableName, string schema = null);

        string GetTableName(Type type);
    }
}
