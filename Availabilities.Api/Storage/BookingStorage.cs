using System.Collections.Generic;
using Availabilities.Resources;

namespace Availabilities.Storage
{
    public class BookingStorage : IStorage<Booking>
    {
        private readonly InMemEntityStore<Booking> store = new InMemEntityStore<Booking>();

        public List<Booking> List()
        {
            return store.List();
        }

        public void Upsert(Booking entity)
        {
            store.Upsert(entity);
        }

        public void Delete(string id)
        {
            store.Delete(id);
        }

        public Booking Get(string id)
        {
            return store.Get(id);
        }
    }
}