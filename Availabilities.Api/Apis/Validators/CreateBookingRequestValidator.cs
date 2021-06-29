using System;
using Availabilities.Apis.ServiceOperations.Bookings;
using Availabilities.Other;
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
            RuleFor(dto => dto.StartUtc)
                .GreaterThanOrEqualTo(DateTime.UtcNow.SubtractMinutes(1))
                .WithMessage("StartUtc must be in the future");
            RuleFor(dto => dto.StartUtc)
                .GreaterThanOrEqualTo(Validations.Availabilities.MinimumAvailability)
                .WithMessage($"StartUtc must be after {Validations.Availabilities.MinimumAvailability:O}");
            RuleFor(dto => dto)
                .Must(dto =>
                    dto.StartUtc.AddMinutes(dto.DurationInMins) <= Validations.Availabilities.MaximumAvailability)
                .WithMessage($"Booking cannot extend past {Validations.Availabilities.MaximumAvailability:O}");
            RuleFor(dto => dto.DurationInMins)
                .Must(duration => duration.IsDivisibleBy(Validations.Bookings.BookingIncrementInMinutes))
                .WithMessage(
                    $"DurationInMins must be in divisions of {Validations.Bookings.BookingIncrementInMinutes}mins");
            RuleFor(dto => dto.DurationInMins)
                .GreaterThanOrEqualTo(Validations.Bookings.MinimumBookingLengthInMinutes)
                .WithMessage(
                    $"DurationInMins must be at least {Validations.Bookings.MinimumBookingLengthInMinutes}mins long");
            RuleFor(dto => dto.DurationInMins)
                .LessThanOrEqualTo(dto => Validations.Bookings.MaximumBookingLengthInMinutes)
                .WithMessage(
                    $"DurationInMins must not be more than {Validations.Bookings.MaximumBookingLengthInMinutes}mins long");
        }
    }
}