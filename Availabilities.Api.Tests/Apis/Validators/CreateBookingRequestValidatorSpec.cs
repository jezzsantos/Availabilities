using System;
using Availabilities.Apis.ServiceOperations.Bookings;
using Availabilities.Apis.Validators;
using Availabilities.Other;
using FluentAssertions;
using ServiceStack.FluentValidation;
using Xunit;

namespace Availabilities.Api.Tests.Apis.Validators
{
    [Trait("Category", "Unit")]
    public class CreateBookingRequestValidatorSpec
    {
        private readonly CreateBookingRequest dto;
        private readonly CreateBookingRequestValidator validator;

        public CreateBookingRequestValidatorSpec()
        {
            this.validator = new CreateBookingRequestValidator();

            var startUtc = DateTime.UtcNow.AddSeconds(1);
            this.dto = new CreateBookingRequest
            {
                StartUtc = startUtc,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription"
            };
        }

        [Fact]
        public void WhenAllPropertiesAreValid_ThenSucceeds()
        {
            this.validator.ValidateAndThrow(this.dto);
        }

        [Fact]
        public void WhenStartUtcIsMissing_TheThrows()
        {
            this.dto.StartUtc = default;
            this.dto.DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes;

            this.validator
                .Invoking(x => x.ValidateAndThrow(this.dto))
                .Should().Throw<ValidationException>()
                .WithMessageLike($"StartUtc must be after {Validations.Availabilities.MinimumAvailability:O}");
        }


        [Fact]
        public void WhenStartUtcIsNearInPast_TheSucceeds()
        {
            this.dto.StartUtc = DateTime.UtcNow.SubtractSeconds(58);
            this.dto.DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes;

            this.validator.ValidateAndThrow(this.dto);
        }

        [Fact]
        public void WhenStartUtcIsTooFarInPast_TheThrows()
        {
            this.dto.StartUtc = DateTime.UtcNow.SubtractMinutes(1).SubtractSeconds(1);
            this.dto.DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes;

            this.validator
                .Invoking(x => x.ValidateAndThrow(this.dto))
                .Should().Throw<ValidationException>()
                .WithMessageLike("StartUtc must be in the future");
        }

        [Fact]
        public void WhenStartUtcIsTooFarBackInTime_TheThrows()
        {
            this.dto.StartUtc = Validations.Availabilities.MinimumAvailability.SubtractMinutes(1);
            this.dto.DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes;

            this.validator
                .Invoking(x => x.ValidateAndThrow(this.dto))
                .Should().Throw<ValidationException>()
                .WithMessageLike($"StartUtc must be after {Validations.Availabilities.MinimumAvailability:O}");
        }

        [Fact]
        public void WhenStartUtcIsTooFarAheadInTime_TheThrows()
        {
            this.dto.StartUtc = Validations.Availabilities.MaximumAvailability;
            this.dto.DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes;

            this.validator
                .Invoking(x => x.ValidateAndThrow(this.dto))
                .Should().Throw<ValidationException>()
                .WithMessageLike($"Booking cannot extend past {Validations.Availabilities.MaximumAvailability:O}");
        }

        [Fact]
        public void WhenDurationInMinsIsTooShort_TheThrows()
        {
            this.dto.StartUtc = DateTime.UtcNow.AddHours(1);
            this.dto.DurationInMins = 5;

            this.validator
                .Invoking(x => x.ValidateAndThrow(this.dto))
                .Should().Throw<ValidationException>()
                .WithMessageLike(
                    $"DurationInMins must be at least {Validations.Bookings.MinimumBookingLengthInMinutes}mins long");
        }

        [Fact]
        public void WhenDurationInMinsIsTooLong_TheThrows()
        {
            this.dto.StartUtc = DateTime.UtcNow.AddHours(1);
            this.dto.DurationInMins = Validations.Bookings.MaximumBookingLengthInMinutes +
                                      Validations.Bookings.BookingIncrementInMinutes;

            this.validator
                .Invoking(x => x.ValidateAndThrow(this.dto))
                .Should().Throw<ValidationException>()
                .WithMessageLike(
                    $"DurationInMins must not be more than {Validations.Bookings.MaximumBookingLengthInMinutes}mins long");
        }

        [Fact]
        public void WhenDurationInMinsIsMissing_TheThrows()
        {
            this.dto.StartUtc = DateTime.UtcNow.AddHours(1);
            this.dto.DurationInMins = default;

            this.validator
                .Invoking(x => x.ValidateAndThrow(this.dto))
                .Should().Throw<ValidationException>()
                .WithMessageLike(
                    $"DurationInMins must be at least {Validations.Bookings.MinimumBookingLengthInMinutes}mins long");
        }
    }
}