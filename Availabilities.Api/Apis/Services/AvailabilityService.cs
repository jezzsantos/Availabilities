using Availabilities.Apis.ServiceOperations.Availabilities;
using Availabilities.Resources;
using Availabilities.Storage;
using ServiceStack;

namespace Availabilities.Apis.Services
{
    internal class AvailabilityService : Service
    {
        private readonly IStorage<Availability> storage;

        public AvailabilityService(IStorage<Availability> storage)
        {
            this.storage = storage;
        }

        public GetAllAvailabilitiesResponse Get(GetAllAvailabilitiesRequest request)
        {
            var availabilities = storage.List();

            return new GetAllAvailabilitiesResponse
            {
                Availabilitites = availabilities
            };
        }
    }
}