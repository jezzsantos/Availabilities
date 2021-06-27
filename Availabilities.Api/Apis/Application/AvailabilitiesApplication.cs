using System;
using Availabilities.Resources;
using Availabilities.Storage;

namespace Availabilities.Apis.Application
{
    internal class AvailabilitiesApplication : IAvailabilitiesApplication
    {
        private readonly IStorage<Availability> storage;

        public AvailabilitiesApplication(IStorage<Availability> storage)
        {
            this.storage = storage;
        }

        public TimeSlot ReserveAvailability(TimeSlot slot)
        {
            //TODO: remember to round up the slot times to the next quarter hour
            //TODO: remember to determine what data needs to be changed to remove this availability
            //TODO: Remember to throw new ResourceConflictException("The booking cannot be made for this time period") if not available
            //TODO: remember to return the actual slot

            throw new NotImplementedException();
        }

        public void ReleaseAvailability(TimeSlot slot)
        {
            //TODO: remember to determine what data needs to be changed to add this availability back

            throw new NotImplementedException();
        }
    }
}