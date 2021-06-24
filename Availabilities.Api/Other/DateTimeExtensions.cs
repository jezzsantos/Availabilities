using System;

namespace Availabilities.Other
{
    public static class DateTimeExtensions
    {
        public static bool HasValue(this DateTime current)
        {
            if (current.Kind == DateTimeKind.Local)
            {
                return current != DateTime.MinValue.ToLocalTime()
                       && current != DateTime.MinValue;
            }

            return current != DateTime.MinValue;
        }

        public static bool HasValue(this DateTime? datum)
        {
            if (!datum.HasValue)
            {
                return false;
            }

            return datum.Value.HasValue();
        }

        public static string ToIso8601(this DateTime dateTime)
        {
            var utcDateTime = dateTime.Kind != DateTimeKind.Utc
                ? dateTime
                : dateTime.ToUniversalTime();

            return utcDateTime.ToString("O");
        }

        public static DateTime FromIso8601(this string value)
        {
            if (!value.HasValue())
            {
                return DateTime.MinValue;
            }

            var dateTime = DateTime.Parse(value);

            return dateTime.HasValue()
                ? dateTime.ToUniversalTime()
                : DateTime.MinValue;
        }
    }
}