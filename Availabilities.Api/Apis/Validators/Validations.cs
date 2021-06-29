using System;

namespace Availabilities.Apis.Validators
{
    public static class Validations
    {
        public static class Bookings
        {
            public const int BookingIncrementInMinutes = 15;
            private const int MaximumBookingIncrements = 12;
            public const int MinimumBookingLengthInMinutes = BookingIncrementInMinutes * 1;
            public const int MaximumBookingLengthInMinutes = BookingIncrementInMinutes * MaximumBookingIncrements;
        }

        public class Availabilities
        {
            public static readonly DateTime MinimumAvailability = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            public static readonly DateTime MaximumAvailability = new DateTime(2050, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        }
    }
}