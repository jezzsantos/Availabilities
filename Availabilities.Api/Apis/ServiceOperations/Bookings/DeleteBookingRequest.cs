using ServiceStack;

namespace Availabilities.Apis.ServiceOperations.Bookings
{
    [Route("/bookings/{Id}", "DELETE")]
    public class DeleteBookingRequest : IReturn<DeleteBookingResponse>
    {
        public string Id { get; set; }
    }
}