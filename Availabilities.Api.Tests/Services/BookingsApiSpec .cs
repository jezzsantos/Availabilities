using System;
using System.Net;
using Availabilities.Apis.ServiceOperations.Bookings;
using Availabilities.Apis.Validators;
using Availabilities.Other;
using Availabilities.Resources;
using Availabilities.Storage;
using FluentAssertions;
using ServiceStack;
using Xunit;

namespace Availabilities.Api.Tests.Services
{
    [Trait("Category", "Integration")]
    [Collection("ThisAssembly")]
    public class BookingsApiSpec : IClassFixture<ApiSpecSetup<TestStartup>>
    {
        private readonly ApiSpecSetup<TestStartup> setup;

        public BookingsApiSpec(ApiSpecSetup<TestStartup> setup)
        {
            this.setup = setup;
            this.setup.Resolve<IStorage<Booking>>().DestroyAll();
            this.setup.Resolve<IStorage<Availability>>().DestroyAll();
        }

        [Fact]
        public void WhenCreateBookingAndAllAvailability_ThenCreatesBookingToNextOrCurrentQuarterHour()
        {
            var start = DateTime.UtcNow;
            var end = start.FifteenMinutesLater();
            var booking = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription"
            }).Booking;

            booking.Id.Should().NotBeEmpty();
            booking.StartUtc.Should().Be(start.ToNextOrCurrentQuarterHour());
            booking.EndUtc.Should().Be(end.ToNextOrCurrentQuarterHour());
            booking.Description.Should().Be("adescription");
        }

        [Fact]
        public void WhenCreateBookingAndExactlyNoAvailability_ThenThrows()
        {
            var start = DateTime.UtcNow;
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            });

            this.setup.Api
                .Invoking(x => x.Post(new CreateBookingRequest
                {
                    StartUtc = start,
                    DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                    Description = "adescription2"
                }))
                .Should().Throw<WebServiceException>()
                .Which.HasStatus(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void WhenCreateBookingAndWithinNoAvailability_ThenThrows()
        {
            var start = DateTime.UtcNow.ToNextQuarterHour();
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes * 4,
                Description = "adescription1"
            });

            this.setup.Api
                .Invoking(x => x.Post(new CreateBookingRequest
                {
                    StartUtc = start.FifteenMinutesLater(),
                    DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                    Description = "adescription2"
                }))
                .Should().Throw<WebServiceException>()
                .Which.HasStatus(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void WhenCreateBookingAndOutsideNoAvailability_ThenThrows()
        {
            var start1 = DateTime.UtcNow.AnHourLater().ToNextQuarterHour();
            var start2 = start1.FifteenMinutesEarlier();
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start1,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            });

            this.setup.Api
                .Invoking(x => x.Post(new CreateBookingRequest
                {
                    StartUtc = start2,
                    DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes * 2,
                    Description = "adescription2"
                }))
                .Should().Throw<WebServiceException>()
                .Which.HasStatus(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void WhenCreateBookingAndNoAvailabilityAtStart_ThenThrows()
        {
            var start = DateTime.UtcNow;
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes * 2,
                Description = "adescription1"
            });

            this.setup.Api
                .Invoking(x => x.Post(new CreateBookingRequest
                {
                    StartUtc = start.FifteenMinutesLater(),
                    DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes * 2,
                    Description = "adescription2"
                }))
                .Should().Throw<WebServiceException>()
                .Which.HasStatus(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void WhenCreateBookingAndNoAvailabilityAtEnd_ThenThrows()
        {
            var start = DateTime.UtcNow.FifteenMinutesLater();
            this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes * 2,
                Description = "adescription1"
            });

            this.setup.Api
                .Invoking(x => x.Post(new CreateBookingRequest
                {
                    StartUtc = start.FifteenMinutesEarlier(),
                    DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes * 2,
                    Description = "adescription2"
                }))
                .Should().Throw<WebServiceException>()
                .Which.HasStatus(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void WhenGetAllBookingsAndNoBookings_ThenReturnsNone()
        {
            var bookings = this.setup.Api.Get(new GetAllBookingsRequest())
                .Bookings;

            bookings.Should().HaveCount(0);
        }

        [Fact]
        public void WhenGetAllBookingsAndSingleBooking_ThenReturnsOne()
        {
            var start = DateTime.UtcNow;
            var booking = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            }).Booking;

            var bookings = this.setup.Api.Get(new GetAllBookingsRequest())
                .Bookings;

            bookings.Should().HaveCount(1);
            bookings[0].Id.Should().Be(booking.Id);
            bookings[0].StartUtc.Should().Be(booking.StartUtc);
            bookings[0].EndUtc.Should().Be(booking.EndUtc);
            bookings[0].Description.Should().Be(booking.Description);
        }

        [Fact]
        public void WhenGetAllBookingsAndAdjacentBookings_ThenReturnsBothOrdered()
        {
            var start1 = DateTime.UtcNow.ToNextQuarterHour();
            var start2 = start1.AddMinutes(Validations.Bookings.MinimumBookingLengthInMinutes);
            var booking1 = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start1,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription1"
            }).Booking;
            var booking2 = this.setup.Api.Post(new CreateBookingRequest
            {
                StartUtc = start2,
                DurationInMins = Validations.Bookings.MinimumBookingLengthInMinutes,
                Description = "adescription2"
            }).Booking;

            var bookings = this.setup.Api.Get(new GetAllBookingsRequest())
                .Bookings;

            bookings.Should().HaveCount(2);
            bookings[0].Id.Should().Be(booking1.Id);
            bookings[1].Id.Should().Be(booking2.Id);
        }
    }
}