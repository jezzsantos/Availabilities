using Availabilities.Apis.Application;
using Availabilities.Resources;
using Availabilities.Storage;
using Moq;
using Xunit;

namespace Availabilities.Api.Tests.Apis.Application
{
    [Trait("Category", "Unit")]
    public class AvailabilitiesApplicationSpec
    {
        private readonly AvailabilitiesApplication application;
        private readonly Mock<IStorage<Availability>> storage;

        public AvailabilitiesApplicationSpec()
        {
            this.storage = new Mock<IStorage<Availability>>();
            this.application = new AvailabilitiesApplication(this.storage.Object);
        }
    }
}