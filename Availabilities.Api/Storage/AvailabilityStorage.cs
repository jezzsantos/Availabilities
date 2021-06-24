using System.Collections.Generic;
using Availabilities.Resources;

namespace Availabilities.Storage
{
    public class AvailabilityStorage : IStorage<Availability>
    {
        private readonly InMemEntityStore<Availability> store = new InMemEntityStore<Availability>();

        public List<Availability> List()
        {
            return store.List();
        }

        public void Upsert(Availability entity)
        {
            store.Upsert(entity);
        }

        public void Delete(string id)
        {
            store.Delete(id);
        }

        public Availability Get(string id)
        {
            return store.Get(id);
        }
    }
}