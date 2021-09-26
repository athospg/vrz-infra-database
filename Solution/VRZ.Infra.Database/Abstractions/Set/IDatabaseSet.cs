using System;

namespace VRZ.Infra.Database.Abstractions.Set
{
    public interface IDatabaseSet<in TEntity> :
        IDatabaseSetClauses<TEntity>,
        IDatabaseSetBuilders<TEntity>,
        IDatabaseSetBulk<TEntity>
    {
        /// <summary>
        ///     Object to string default converter.
        /// </summary>
        Func<object, string> Converter { get; }

        string GetTable();
    }
}
