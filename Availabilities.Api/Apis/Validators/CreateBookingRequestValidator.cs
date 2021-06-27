using System;
using Availabilities.Apis.ServiceOperations.Bookings;
using ServiceStack.FluentValidation;

namespace Availabilities.Apis.Validators
{
    // ReSharper disable once UnusedType.Global
    internal class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
    {
        public CreateBookingRequestValidator()
        {
            RuleFor(dto => dto.StartUtc)
                .GreaterThanOrEqualTo(Validations.Availabilities.MinimumAvailability)
                .WithMessage($"StartUtc must be after {Validations.Availabilities.MinimumAvailability:O}");
            RuleFor(dto => dto.EndUtc)
                .LessThanOrEqualTo(Validations.Availabilities.MaximumAvailability)
                .WithMessage($"EndUtc must be before {Validations.Availabilities.MaximumAvailability:O}");
            RuleFor(dto => dto.StartUtc)
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("StartUtc must be in the future");
            RuleFor(dto => dto.StartUtc)
                .NotEqual(dto => dto.EndUtc)
                .WithMessage("StartUtc cannot be same as EndUtc");
            RuleFor(dto => dto.StartUtc)
                .LessThan(dto => dto.EndUtc)
                .WithMessage("StartUtc must be before the EndUtc");
            RuleFor(dto => dto.EndUtc)
                .GreaterThanOrEqualTo(
                    dto => dto.StartUtc.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes))
                .WithMessage($"EndUtc must be at least {Validations.Bookings.MinimumBookingLengthInMinutes}mins long");
            RuleFor(dto => dto.EndUtc)
                .LessThanOrEqualTo(dto => dto.StartUtc.AddMinutes(Validations.Bookings.MaximumBookingLengthInMinutes))
                .WithMessage(
                    $"EndUtc must not be more than {Validations.Bookings.MaximumBookingLengthInMinutes}mins long");
        }
    }
}