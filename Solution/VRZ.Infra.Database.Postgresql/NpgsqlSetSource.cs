using System;
using System.Collections.Concurrent;
using System.Reflection;
using VRZ.Infra.Database.Abstractions.Context;
using VRZ.Infra.Database.Abstractions.Set;

namespace VRZ.Infra.Database.Postgresql
{
    public class NpgsqlSetSource : IDatabaseSetSource
    {
        private static readonly MethodInfo _genericCreateSet =
            typeof(NpgsqlSetSource).GetTypeInfo().GetDeclaredMethod(nameof(CreateSetFactory));

        private readonly ConcurrentDictionary<(Type Type, string Name), Func<IDatabaseContext, string, object>>
            _cache = new();


        public virtual object Create(IDatabaseContext context, Type type)
        {
            return CreateCore(context, type, null, _genericCreateSet);
        }

        public virtual object Create(IDatabaseContext context, string name, Type type)
        {
            return CreateCore(context, type, name, _genericCreateSet);
        }

        private object CreateCore(IDatabaseContext context, Type type, string name, MethodInfo createMethod)
        {
            return _cache.GetOrAdd((type, name),
                t => (Func<IDatabaseContext, string, object>)createMethod
                    .MakeGenericMethod(t.Type)
                    .Invoke(null, null))(context, name);
        }

        private static Func<IDatabaseContext, string, object> CreateSetFactory<TEntity>()
        {
            return (c, name) => new NpgsqlSet<TEntity>(c, name);
        }
    }
}
