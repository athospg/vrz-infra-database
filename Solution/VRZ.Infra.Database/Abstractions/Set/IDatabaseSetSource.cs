using System;
using VRZ.Infra.Database.Abstractions.Context;

namespace VRZ.Infra.Database.Abstractions.Set
{
    public interface IDatabaseSetSource
    {
        object Create(IDatabaseContext context, Type type);

        object Create(IDatabaseContext context, string name, Type type);
    }
}
