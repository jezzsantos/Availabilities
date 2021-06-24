using System;
using Availabilities.Resources;
using Availabilities.Storage;

namespace Availabilities.Apis.Application
{
    internal class AvailabilitiesService : IAvailabilitiesService
    {
        private readonly IStorage<Availability> storage;

        public AvailabilitiesService(IStorage<Availability> storage)
        {
            this.storage = storage;
        }

        public bool IsAvailable(DateTime start, DateTime end)
        {
            //TODO: remember to check the data for spare availability 

            throw new NotImplementedException();
        }

        public void ReserveAvailability(DateTime start, DateTime end)
        {
            //TODO: remember to determine what data needs to be changed to remove this availability

            throw new NotImplementedException();
        }

        public void ReleaseAvailability(DateTime start, DateTime end)
        {
            //TODO: remember to determine what data needs to be changed to add this availability back

            throw new NotImplementedException();
        }
    }
}