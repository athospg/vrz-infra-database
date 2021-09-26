using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRZ.Infra.Database.Abstractions.Context;
using VRZ.Infra.Database.Utils;

namespace VRZ.Infra.Database.Abstractions.Set.Internal
{
    public class DatabaseSetFinder : IDatabaseSetFinder
    {
        private readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> _cache = new();

        public virtual IReadOnlyList<PropertyInfo> FindSets(Type contextType)
        {
            return _cache.GetOrAdd(contextType, FindSetsNonCached);
        }

        private static PropertyInfo[] FindSetsNonCached(Type contextType)
        {
            return contextType.GetRuntimeProperties()
                .Where(p => !p.IsStatic()
                            && !p.GetIndexParameters().Any()
                            && p.DeclaringType != typeof(DatabaseContext)
                            && p.PropertyType.GetTypeInfo().IsGenericType
                            && p.PropertyType.GetGenericTypeDefinition() == typeof(DatabaseSet<>))
                .OrderBy(p => p.Name)
                .ToArray();
        }
    }
}
