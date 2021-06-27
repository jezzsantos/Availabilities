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
            //TODO: remember to round up the slot times to the nearest quarter hour
            //TODO: remember to determine what data needs to be changed to remove this slot from the current availability slots
            //TODO: Remember to throw new ResourceConflictException("The booking cannot be made for this time period") if there is not enough availability to cover it
            //TODO: remember to return the actual slot that was made available

            throw new NotImplementedException();
        }

        public void ReleaseAvailability(TimeSlot slot)
        {
            //TODO: remember to determine what data needs to be changed to add this availability slot back into the current set of availability.
            //TODO: remember that availability is represented in contiguous blocks of time, never adjacent to each other.

            throw new NotImplementedException();
        }
    }
}