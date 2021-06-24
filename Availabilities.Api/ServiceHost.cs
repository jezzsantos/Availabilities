using System;
using System.Reflection;
using Availabilities.Apis.Application;
using Availabilities.Other;
using Availabilities.Resources;
using Availabilities.Storage;
using Funq;
using ServiceStack;
using ServiceStack.Text;

namespace Availabilities
{
    public class ServiceHost : AppHostBase
    {
        private static readonly Assembly[] AssembliesContainingServicesAndDependencies =
        {
            typeof(Startup).Assembly
        };

        public ServiceHost() : base("Availabilities Website", AssembliesContainingServicesAndDependencies)
        {
        }

        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                DebugMode = true,
                HandlerFactoryPath = "api",
                DefaultRedirectPath = "/metadata",
                ReturnsInnerException = true
            });

            JsConfig.DateHandler = DateHandler.ISO8601;
            JsConfig.AssumeUtc = true;
            JsConfig.AlwaysUseUtc = true;
            JsConfig.ExcludeTypeInfo = true;
            JsConfig.IncludeNullValues = false;
            JsConfig.TextCase = TextCase.CamelCase;

            JsConfig<DateTime>.SerializeFn = dt => !dt.HasValue()
                ? null
                : dt.ToIso8601();
            JsConfig<DateTime?>.SerializeFn = dt => !dt.HasValue || !dt.Value.HasValue()
                ? null
                : dt.Value.ToIso8601();

            container.AddSingleton<IStorage<Availability>, AvailabilityStorage>();
            container.AddSingleton<IStorage<Booking>, BookingStorage>();
            container.AddSingleton<IAvailabilitiesService, AvailabilitiesService>();
        }
    }
}