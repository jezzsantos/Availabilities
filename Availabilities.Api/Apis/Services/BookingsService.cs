using Availabilities.Apis.Application;
using Availabilities.Apis.ServiceOperations.Bookings;
using Availabilities.Resources;
using Availabilities.Storage;
using ServiceStack;

namespace Availabilities.Apis.Services
{
    internal class BookingsService : Service
    {
        private readonly IAvailabilitiesApplication availabilitiesApplication;
        private readonly IStorage<Booking> storage;

        public BookingsService(IStorage<Booking> storage, IAvailabilitiesApplication availabilitiesApplication)
        {
            this.storage = storage;
            this.availabilitiesApplication = availabilitiesApplication;
        }

        public CreateBookingResponse Post(CreateBookingRequest request)
        {
            var requestedSlot = new TimeSlot(request.StartUtc, request.EndUtc);

            var availableSlot = this.availabilitiesApplication.ReserveAvailability(requestedSlot);

            var booking = new Booking
            {
                StartUtc = availableSlot.Start,
                EndUtc = availableSlot.End,
                Description = request.Description
            };
            this.storage.Upsert(booking);

            return new CreateBookingResponse
            {
                Booking = booking
            };
        }

        public GetAllBookingsResponse Get(GetAllBookingsRequest request)
        {
            var bookings = this.storage.List();

            return new GetAllBookingsResponse
            {
                Bookings = bookings
            };
        }

        public DeleteBookingResponse Delete(DeleteBookingRequest request)
        {
            var booking = this.storage.Get(request.Id);
            this.storage.Delete(request.Id);

            var slot = new TimeSlot(booking.StartUtc, booking.EndUtc);
            this.availabilitiesApplication.ReleaseAvailability(slot);

            return new DeleteBookingResponse();
        }
    }
}