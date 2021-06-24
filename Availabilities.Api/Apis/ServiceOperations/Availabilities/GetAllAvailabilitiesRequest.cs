using ServiceStack;

namespace Availabilities.Apis.ServiceOperations.Availabilities
{
    [Route("/availabilitites", "GET")]
    public class GetAllAvailabilitiesRequest : IReturn<GetAllAvailabilitiesResponse>, IGet
    {
    }
}