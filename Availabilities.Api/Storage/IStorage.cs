using System.Collections.Generic;
using Availabilities.Resources;

namespace Availabilities.Storage
{
    public interface IStorage<TEntity> where TEntity : IHasIdentifier, new()
    {
        List<TEntity> List();
        void Upsert(TEntity entity);
        void Delete(string id);
        TEntity Get(string id);
        void DestroyAll();
    }
}