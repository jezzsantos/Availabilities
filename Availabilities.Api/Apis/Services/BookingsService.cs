using Availabilities.Apis.Application;
using Availabilities.Apis.ServiceOperations.Bookings;
using Availabilities.Other;
using Availabilities.Resources;
using Availabilities.Storage;
using ServiceStack;

namespace Availabilities.Apis.Services
{
    internal class BookingsService : Service
    {
        private readonly IAvailabilitiesService availabilitiesService;
        private readonly IStorage<Booking> storage;

        public BookingsService(IStorage<Booking> storage, IAvailabilitiesService availabilitiesService)
        {
            this.storage = storage;
            this.availabilitiesService = availabilitiesService;
        }

        public CreateBookingResponse Post(CreateBookingRequest request)
        {
            var booking = new Booking
            {
                StartUtc = request.StartUtc,
                EndUtc = request.EndUtc,
                Description = request.Description
            };

            var isAvailable = availabilitiesService.IsAvailable(request.StartUtc, request.EndUtc);
            if (!isAvailable)
            {
                throw new ResourceConflictException("The booking cannot be made for this time period");
            }

            availabilitiesService.ReserveAvailability(request.StartUtc, request.EndUtc);

            storage.Upsert(booking);

            return new CreateBookingResponse
            {
                Booking = booking
            };
        }

        public GetAllBookingsResponse Post(GetAllBookingsRequest request)
        {
            var bookings = storage.List();

            return new GetAllBookingsResponse
            {
                Bookings = bookings
            };
        }

        public DeleteBookingResponse Post(DeleteBookingRequest request)
        {
            var booking = storage.Get(request.Id);

            storage.Delete(request.Id);

            availabilitiesService.ReleaseAvailability(booking.StartUtc, booking.EndUtc);

            return new DeleteBookingResponse();
        }
    }
}