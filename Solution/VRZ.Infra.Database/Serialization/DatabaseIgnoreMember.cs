using System;

namespace VRZ.Infra.Database.Serialization
{
    /// <summary>
    ///     <see cref="Attribute" /> used to ignore model member on database operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DatabaseIgnoreMember : Attribute
    {
    }
}
