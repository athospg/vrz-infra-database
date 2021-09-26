using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Humanizer;
using VRZ.Infra.Database.Abstractions.Set;
using VRZ.Infra.Database.Abstractions.Set.Internal;

namespace VRZ.Infra.Database.Abstractions.Context
{
    public abstract class DatabaseContext : IDatabaseContext
    {
        protected readonly string ConnectionString;

        private readonly IDatabaseSetSource _databaseSetSource;
        private readonly IDatabaseSetFinder _databaseSetFinder;

        private readonly string _defaultSchema;


        protected DatabaseContext(IDatabaseSetSource databaseSetSource, string connectionString,
            string defaultSchema = null)
        {
            _databaseSetSource = databaseSetSource;
            ConnectionString = connectionString;
            _defaultSchema = defaultSchema;

            _databaseSetFinder = new DatabaseSetFinder();

            InitializeSets();
        }

        private void InitializeSets()
        {
            foreach (var propertyInfo in _databaseSetFinder.FindSets(GetType()).Where(p => p.SetMethod != null))
            {
                SetEntityTable(propertyInfo.PropertyType.GenericTypeArguments.Single(), propertyInfo.Name);

                var set = GetOrAddSet(_databaseSetSource, propertyInfo.PropertyType.GenericTypeArguments.Single());
                GetType().GetProperty(propertyInfo.Name)?.SetValue(this, set, null);
            }
        }


        #region Database converter

        /// <summary>
        ///     Object to string default converter.
        /// </summary>
        public abstract Func<object, string> Converter { get; }

        #endregion

        #region Database connection

        public virtual IDbConnection Connection => GetNewConnection();

        protected abstract IDbConnection GetNewConnection();

        #endregion

        #region Database operations

        public virtual async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null,
            CancellationToken token = new())
        {
            return await Connection.QueryAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout,
                commandType, cancellationToken: token));
        }

        public virtual async Task<int> ExecuteAsync(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null,
            CancellationToken token = new())
        {
            return await Connection.ExecuteAsync(new CommandDefinition(sql, param, transaction, commandTimeout,
                commandType, cancellationToken: token));
        }

        public virtual async Task<T> ExecuteScalarAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null,
            CancellationToken token = new())
        {
            return await Connection.ExecuteScalarAsync<T>(new CommandDefinition(sql, param, transaction, commandTimeout,
                commandType, cancellationToken: token));
        }

        #endregion

        #region Database sets

        private IDictionary<(Type Type, string Name), object> _sets;

        private object GetOrAddSet(IDatabaseSetSource source, Type type)
        {
            _sets ??= new Dictionary<(Type Type, string Name), object>();

            if (!_sets.TryGetValue((type, null), out var set))
            {
                set = source.Create(this, type);
                _sets[(type, null)] = set;
            }

            return set;
        }

        private object GetOrAddSet(IDatabaseSetSource source, string entityTypeName, Type type)
        {
            _sets ??= new Dictionary<(Type Type, string Name), object>();

            if (!_sets.TryGetValue((type, entityTypeName), out var set))
            {
                set = source.Create(this, entityTypeName, type);
                _sets[(type, entityTypeName)] = set;
            }

            return set;
        }

        /// <summary>
        ///     Creates a <see cref="IDatabaseSet{TEntity}" /> that can be used to query and save instances
        ///     of <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity"> The type of entity for which a set should be returned. </typeparam>
        /// <returns> A set for the given entity type. </returns>
        public virtual IDatabaseSet<TEntity> Set<TEntity>()
        {
            return (IDatabaseSet<TEntity>)GetOrAddSet(_databaseSetSource, typeof(TEntity));
        }

        /// <summary>
        ///     Creates a <see cref="IDatabaseSet{TEntity}" /> that can be used to query and save instances
        ///     of <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity"> The type of entity for which a set should be returned. </typeparam>
        /// <returns> A set for the given entity type. </returns>
        public virtual IDatabaseSet<TEntity> Set<TEntity>(string name)
        {
            return (IDatabaseSet<TEntity>)GetOrAddSet(_databaseSetSource, name, typeof(TEntity));
        }


        private IDictionary<Type, string> _setsSchemas;
        private IDictionary<Type, string> _setsTables;

        public void SetEntityTable<TEntity>(string tableName, string schema = null)
        {
            SetEntityTable(typeof(TEntity), tableName, schema);
        }

        public void SetEntityTable(Type type, string tableName, string schema = null)
        {
            _setsTables ??= new Dictionary<Type, string>();
            _setsTables[type] = tableName;

            _setsSchemas ??= new Dictionary<Type, string>();
            _setsSchemas[type] = schema ?? _defaultSchema;
        }

        private string GetOrAddSetTable(Type type)
        {
            _setsTables ??= new Dictionary<Type, string>();

            if (!_setsTables.TryGetValue(type, out var setTable))
            {
                setTable = type.Name.Pluralize(false);
                _setsTables[type] = setTable;
            }

            return setTable;
        }

        private string GetOrAddSetSchema(Type type)
        {
            _setsSchemas ??= new Dictionary<Type, string>();

            if (!_setsSchemas.TryGetValue(type, out var setSchema))
            {
                setSchema = _defaultSchema;
                _setsSchemas[type] = _defaultSchema;
            }

            return setSchema;
        }

        public string GetTableName(Type type)
        {
            var schema = GetOrAddSetSchema(type);
            var tableName = GetOrAddSetTable(type);

            return string.IsNullOrWhiteSpace(schema) ? tableName : $"{schema}.{tableName}";
        }

        #endregion
    }
}
