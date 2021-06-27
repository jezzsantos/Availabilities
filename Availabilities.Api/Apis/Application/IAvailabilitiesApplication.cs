using Availabilities.Resources;

namespace Availabilities.Apis.Application
{
    public interface IAvailabilitiesApplication
    {
        TimeSlot ReserveAvailability(TimeSlot slot);
        void ReleaseAvailability(TimeSlot slot);
    }
}