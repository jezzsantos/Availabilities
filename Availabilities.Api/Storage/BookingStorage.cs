using System.Collections.Generic;
using Availabilities.Resources;

namespace Availabilities.Storage
{
    public class BookingStorage : IStorage<Booking>
    {
        private readonly InMemEntityStore<Booking> store = new InMemEntityStore<Booking>();

        public List<Booking> List()
        {
            return this.store.List();
        }

        public void Upsert(Booking entity)
        {
            this.store.Upsert(entity);
        }

        public void Delete(string id)
        {
            this.store.Delete(id);
        }

        public Booking Get(string id)
        {
            return this.store.Get(id);
        }

        public void DestroyAll()
        {
            this.store.DestroyAll();
        }
    }
}