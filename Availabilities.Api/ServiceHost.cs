using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Availabilities.Apis.Application;
using Availabilities.Other;
using Availabilities.Resources;
using Availabilities.Storage;
using Funq;
using ServiceStack;
using ServiceStack.FluentValidation;
using ServiceStack.Text;
using ServiceStack.Validation;

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
                ReturnsInnerException = true,
                MapExceptionToStatusCode = new Dictionary<Type, int>
                {
                    {typeof(ValidationException), (int) HttpStatusCode.BadRequest},
                    {typeof(ArgumentException), (int) HttpStatusCode.BadRequest},
                    {typeof(ResourceNotFoundException), (int) HttpStatusCode.NotFound},
                    {typeof(ResourceConflictException), (int) HttpStatusCode.Conflict}
                }
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

            Plugins.Add(new ValidationFeature());
            container.RegisterValidators();

            container.AddSingleton<IStorage<Availability>, AvailabilityStorage>();
            container.AddSingleton<IStorage<Booking>, BookingStorage>();
            container.AddSingleton<IAvailabilitiesApplication, AvailabilitiesApplication>();
        }
    }
}