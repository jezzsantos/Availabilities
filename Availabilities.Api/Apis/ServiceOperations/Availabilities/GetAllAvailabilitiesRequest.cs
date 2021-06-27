using ServiceStack;

namespace Availabilities.Apis.ServiceOperations.Availabilities
{
    [Route("/availabilities", "GET")]
    public class GetAllAvailabilitiesRequest : IReturn<GetAllAvailabilitiesResponse>, IGet
    {
    }
}