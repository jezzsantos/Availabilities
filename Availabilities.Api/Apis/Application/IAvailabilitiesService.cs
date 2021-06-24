using System;

namespace Availabilities.Apis.Application
{
    public interface IAvailabilitiesService
    {
        bool IsAvailable(DateTime start, DateTime end);
        void ReserveAvailability(DateTime start, DateTime end);
        void ReleaseAvailability(DateTime start, DateTime end);
    }
}