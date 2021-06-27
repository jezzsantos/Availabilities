using System.Collections.Generic;
using Availabilities.Apis.Validators;
using Availabilities.Resources;

namespace Availabilities.Storage
{
    public class AvailabilityStorage : IStorage<Availability>
    {
        private readonly InMemEntityStore<Availability> store = new InMemEntityStore<Availability>();

        public AvailabilityStorage()
        {
            CreateInitialAvailability();
        }

        public List<Availability> List()
        {
            return this.store.List();
        }

        public void Upsert(Availability entity)
        {
            this.store.Upsert(entity);
        }

        public void Delete(string id)
        {
            this.store.Delete(id);
        }

        public Availability Get(string id)
        {
            return this.store.Get(id);
        }

        public void DestroyAll()
        {
            this.store.DestroyAll();
            CreateInitialAvailability();
        }

        private void CreateInitialAvailability()
        {
            this.store.Upsert(new Availability
            {
                StartUtc = Validations.Availabilities.MinimumAvailability,
                EndUtc = Validations.Availabilities.MaximumAvailability
            });
        }
    }
}