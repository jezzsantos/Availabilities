using System.Collections.Generic;
using Availabilities.Resources;

namespace Availabilities.Apis.ServiceOperations.Bookings
{
    public class GetAllBookingsResponse
    {
        public List<Booking> Bookings { get; set; }
    }
}