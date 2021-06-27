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
            return this.store
                .Select(pair => pair.Value)
                .ToList();
        }

        public void Upsert(TEntity entity)
        {
            if (!entity.Id.HasValue())
            {
                entity.Id = GenerateEntityId();
            }

            if (this.store.ContainsKey(entity.Id))
            {
                this.store[entity.Id] = entity;
            }
            else
            {
                this.store.Add(entity.Id, entity);
            }
        }

        public void Delete(string id)
        {
            if (!id.HasValue())
            {
                throw new ResourceNotFoundException();
            }

            if (this.store.ContainsKey(id))
            {
                this.store.Remove(id);
            }
        }

        public TEntity Get(string id)
        {
            if (!id.HasValue())
            {
                throw new ResourceNotFoundException();
            }

            if (!this.store.ContainsKey(id))
            {
                throw new ResourceNotFoundException();
            }

            return this.store[id];
        }

        public void DestroyAll()
        {
            this.store.Clear();
        }

        private static string GenerateEntityId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}