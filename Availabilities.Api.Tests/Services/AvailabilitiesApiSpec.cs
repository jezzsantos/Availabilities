using System;
using System.Linq;
using Availabilities.Apis.ServiceOperations.Availabilities;
using Availabilities.Apis.ServiceOperations.Bookings;
using Availabilities.Apis.Validators;
using Availabilities.Other;
using Availabilities.Resources;
using Availabilities.Storage;
using FluentAssertions;
using Xunit;

namespace Availabilities.Api.Tests.Services
{
    [Trait("Category", "Integration")]
    [Collection("ThisAssembly")]
    public class AvailabilitiesApiSpec : IClassFixture<ApiSpecSetup<TestStartup>>
    {
        private readonly Availability originalAvailability;
        private readonly ApiSpecSetup<TestStartup> setup;

        public AvailabilitiesApiSpec(ApiSpecSetup<TestStartup> setup)
        {
            this.setup = setup;
            this.setup.Resolve<IStorage<Booking>>().DestroyAll();
            this.setup.Resolve<IStorage<Availability>>().DestroyAll();


            this.originalAvailability = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities.FirstOrDefault();
        }

        [Fact]
        public void WhenGetAllAvailabilitiesAndNoBookings_ThenReturnInitialSlot()
        {
            var availabilities = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities;

            availabilities.Should().HaveCount(1);
            availabilities[0].StartUtc.Should().Be(Validations.Availabilities.MinimumAvailability);
            availabilities[0].EndUtc.Should().Be(Validations.Availabilities.MaximumAvailability);
        }

        [Fact]
        public void WhenGetAllAvailabilitiesAndCreateBooking_ThenReturnsBothSlotsOrdered()
        {
            var start = DateTime.UtcNow.ToNextOrCurrentQuarterHour();
            var end = start.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            });

            var availabilities = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities;

            availabilities.Should().HaveCount(2);
            availabilities[0].Id.Should().Be(this.originalAvailability.Id);
            availabilities[0].StartUtc.Should().Be(Validations.Availabilities.MinimumAvailability);
            availabilities[0].EndUtc.Should().Be(start);
            availabilities[1].StartUtc.Should().Be(end);
            availabilities[1].EndUtc.Should().Be(Validations.Availabilities.MaximumAvailability);
        }

        [Fact]
        public void WhenGetAllAvailabilitiesAndCreateAdjacentBookings_ThenReturnsBothSlotsOrdered()
        {
            var start1 = DateTime.UtcNow.ToNextOrCurrentQuarterHour();
            var end1 = start1.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start1,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            });
            var start2 = end1;
            var end2 = start2.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start2,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription2"
            });

            var availabilities = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities;

            availabilities.Should().HaveCount(2);
            availabilities[0].Id.Should().Be(this.originalAvailability.Id);
            availabilities[0].StartUtc.Should().Be(Validations.Availabilities.MinimumAvailability);
            availabilities[0].EndUtc.Should().Be(start1);
            availabilities[1].StartUtc.Should().Be(end2);
            availabilities[1].EndUtc.Should().Be(Validations.Availabilities.MaximumAvailability);
        }

        [Fact]
        public void WhenGetAllAvailabilitiesAndCreateDetachedBookings_ThenReturnsAllThreeSlotsOrdered()
        {
            var start1 = DateTime.UtcNow.ToNextOrCurrentQuarterHour();
            var end1 = start1.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start1,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            });
            var start2 = end1.AnHourLater();
            var end2 = start2.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start2,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription2"
            });

            var availabilities = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities;

            availabilities.Should().HaveCount(3);
            availabilities[0].Id.Should().Be(this.originalAvailability.Id);
            availabilities[0].StartUtc.Should().Be(Validations.Availabilities.MinimumAvailability);
            availabilities[0].EndUtc.Should().Be(start1);
            availabilities[1].StartUtc.Should().Be(end1);
            availabilities[1].EndUtc.Should().Be(start2);
            availabilities[2].StartUtc.Should().Be(end2);
            availabilities[2].EndUtc.Should().Be(Validations.Availabilities.MaximumAvailability);
        }

        [Fact]
        public void WhenGetAllAvailabilitiesAndDeleteCreatedBooking_ThenReturnsOriginalSlot()
        {
            var start = DateTime.UtcNow.ToNextQuarterHour();
            var booking = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            }).Booking;
            this.setup.Api.Delete(new DeleteBookingRequest
            {
                Id = booking.Id
            });

            var availabilities = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities;

            availabilities.Should().HaveCount(1);
            availabilities[0].Id.Should().Be(this.originalAvailability.Id);
            availabilities[0].StartUtc.Should().Be(Validations.Availabilities.MinimumAvailability);
            availabilities[0].EndUtc.Should().Be(Validations.Availabilities.MaximumAvailability);
        }

        [Fact]
        public void WhenGetAllAvailabilitiesAndDeleteFirstAdjacentBooking_ThenReturnsTwoSlots()
        {
            var start1 = DateTime.UtcNow.ToNextQuarterHour();
            var end1 = start1.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            var booking1 = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start1,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            }).Booking;
            var start2 = end1;
            var end2 = start2.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start2,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription2"
            });

            var lastAvailability = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities.Last();
            this.setup.Api.Delete(new DeleteBookingRequest
            {
                Id = booking1.Id
            });

            var availabilities = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities;

            availabilities.Should().HaveCount(2);
            availabilities[0].Id.Should().Be(this.originalAvailability.Id);
            availabilities[0].StartUtc.Should().Be(Validations.Availabilities.MinimumAvailability);
            availabilities[0].EndUtc.Should().Be(start2);
            availabilities[1].Id.Should().Be(lastAvailability.Id);
            availabilities[1].StartUtc.Should().Be(end2);
            availabilities[1].EndUtc.Should().Be(Validations.Availabilities.MaximumAvailability);
        }

        [Fact]
        public void WhenGetAllAvailabilitiesAndDeleteSecondAdjacentBooking_ThenReturnsTwoSlots()
        {
            var start1 = DateTime.UtcNow.ToNextQuarterHour();
            var end1 = start1.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start1,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            });
            var start2 = end1;
            var booking2 = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start2,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription2"
            }).Booking;

            var lastAvailability = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities.Last();
            this.setup.Api.Delete(new DeleteBookingRequest
            {
                Id = booking2.Id
            });

            var availabilities = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities;

            availabilities.Should().HaveCount(2);
            availabilities[0].Id.Should().Be(this.originalAvailability.Id);
            availabilities[0].StartUtc.Should().Be(Validations.Availabilities.MinimumAvailability);
            availabilities[0].EndUtc.Should().Be(start1);
            availabilities[1].Id.Should().Be(lastAvailability.Id);
            availabilities[1].StartUtc.Should().Be(end1);
            availabilities[1].EndUtc.Should().Be(Validations.Availabilities.MaximumAvailability);
        }

        [Fact]
        public void WhenGetAllAvailabilitiesAndDeleteDetachedBookings_ThenReturnsOneSlot()
        {
            var start1 = DateTime.UtcNow.ToNextQuarterHour();
            var booking1 = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start1,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            }).Booking;
            var start2 = start1.AddDays(1);
            var booking2 = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start2,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription2"
            }).Booking;
            this.setup.Api.Delete(new DeleteBookingRequest
            {
                Id = booking1.Id
            });
            this.setup.Api.Delete(new DeleteBookingRequest
            {
                Id = booking2.Id
            });

            var availabilities = this.setup.Api.Get(new GetAllAvailabilitiesRequest())
                .Availabilities;

            availabilities.Should().HaveCount(1);
            availabilities[0].Id.Should().Be(this.originalAvailability.Id);
            availabilities[0].StartUtc.Should().Be(Validations.Availabilities.MinimumAvailability);
            availabilities[0].EndUtc.Should().Be(Validations.Availabilities.MaximumAvailability);
        }
    }
}