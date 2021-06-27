using System;

namespace Availabilities.Apis.Validators
{
    public static class Validations
    {
        public static class Bookings
        {
            public const int MinimumBookingLengthInMinutes = 15;
            public const int MaximumBookingLengthInMinutes = 180;
        }

        public class Availabilities
        {
            public static readonly DateTime MinimumAvailability = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            public static readonly DateTime MaximumAvailability = new DateTime(2050, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        }
    }
}