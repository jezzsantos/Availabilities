using System;
using ServiceStack;

namespace Availabilities.Apis.ServiceOperations.Bookings
{
    [Route("/bookings", "POST")]
    public class CreateBookingRequest : IReturn<CreateBookingResponse>, IPost
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string Description { get; set; }
    }
}