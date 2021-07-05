using ServiceStack;

namespace Availabilities.Api.Tests
{
    public static class ApiClient
    {
        public static JsonServiceClient Create(string serviceUrl)
        {
            return new JsonServiceClient($"{serviceUrl.WithTrailingSlash()}api/");
        }
    }
}