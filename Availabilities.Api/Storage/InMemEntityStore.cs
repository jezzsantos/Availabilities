using System;
using System.Collections.Generic;
using System.Linq;
using Availabilities.Other;
using Availabilities.Resources;

namespace Availabilities.Storage
{
    internal sealed class InMemEntityStore<TEntity> where TEntity : IHasIdentifier, new()
    {
        private readonly Dictionary<string, TEntity> store = new Dictionary<string, TEntity>();

        public List<TEntity> List()
        {
            return store
                .Select(pair => pair.Value)
                .ToList();
        }

        public void Upsert(TEntity entity)
        {
            if (!entity.Id.HasValue())
            {
                entity.Id = GenerateEntityId();
            }

            if (store.ContainsKey(entity.Id))
            {
                store[entity.Id] = entity;
            }
            else
            {
                store.Add(entity.Id, entity);
            }
        }

        public void Delete(string id)
        {
            if (!id.HasValue())
            {
                throw new ResourceNotFoundException();
            }

            if (store.ContainsKey(id))
            {
                store.Remove(id);
            }
        }

        public TEntity Get(string id)
        {
            if (!id.HasValue())
            {
                throw new ResourceNotFoundException();
            }

            if (!store.ContainsKey(id))
            {
                throw new ResourceNotFoundException();
            }

            return store[id];
        }

        private static string GenerateEntityId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}