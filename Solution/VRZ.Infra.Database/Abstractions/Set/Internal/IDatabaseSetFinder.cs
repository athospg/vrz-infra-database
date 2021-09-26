using System;
using System.Collections.Generic;
using System.Reflection;

namespace VRZ.Infra.Database.Abstractions.Set.Internal
{
    public interface IDatabaseSetFinder
    {
        IReadOnlyList<PropertyInfo> FindSets(Type contextType);
    }
}
