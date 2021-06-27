using Availabilities.Apis.ServiceOperations.Availabilities;
using Availabilities.Resources;
using Availabilities.Storage;
using ServiceStack;

namespace Availabilities.Apis.Services
{
    internal class AvailabilitiesService : Service
    {
        private readonly IStorage<Availability> storage;

        public AvailabilitiesService(IStorage<Availability> storage)
        {
            this.storage = storage;
        }

        public GetAllAvailabilitiesResponse Get(GetAllAvailabilitiesRequest request)
        {
            var availabilities = this.storage.List();

            return new GetAllAvailabilitiesResponse
            {
                Availabilities = availabilities
            };
        }
    }
}