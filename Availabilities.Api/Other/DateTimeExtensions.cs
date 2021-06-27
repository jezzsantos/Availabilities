using System;

namespace Availabilities.Other
{
    public static class DateTimeExtensions
    {
        public enum DateRounding
        {
            Nearest = 0,
            Up = 1,
            Down = 2
        }

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

        public static DateTime SubtractMinutes(this DateTime value, int minutes)
        {
            return value.AddMinutes(-minutes);
        }

        public static DateTime FiveMinutesLater(this DateTime value)
        {
            return value.AddMinutes(5);
        }

        public static DateTime FiveMinutesEarlier(this DateTime value)
        {
            return value.SubtractMinutes(5);
        }

        public static DateTime AnHourLater(this DateTime value)
        {
            return value.AddHours(1);
        }

        public static DateTime AnHourEarlier(this DateTime value)
        {
            return value.AddHours(-1);
        }

        public static DateTime ToNearestQuarterHour(this DateTime current)
        {
            if (current.Minute == 0 || current.Minute.IsDivisibleBy(15))
            {
                return current.RoundTo(TimeSpan.FromMinutes(1), DateRounding.Down);
            }

            return current.ToNextQuarterHour();
        }


        public static DateTime ToNextQuarterHour(this DateTime current)
        {
            var interval = TimeSpan.FromMinutes(15);
            return current.RoundTo(interval, DateRounding.Up);
        }

        private static DateTime RoundTo(this DateTime current, TimeSpan interval, DateRounding rounding)
        {
            if (interval == TimeSpan.Zero)
            {
                return current;
            }

            switch (rounding)
            {
                case DateRounding.Down:
                    return current.RoundDown(interval);

                case DateRounding.Up:
                    return current.RoundUp(interval);

                default:
                    return current.RoundToNearest(interval);
            }
        }

        private static DateTime RoundUp(this DateTime dateTime, TimeSpan interval)
        {
            if (dateTime == DateTime.MaxValue)
            {
                return dateTime;
            }

            var delta = (interval.Ticks - dateTime.Ticks % interval.Ticks) % interval.Ticks;
            var additional = delta == 0 ? interval.Ticks : delta;

            return new DateTime(dateTime.Ticks + additional, dateTime.Kind);
        }

        private static DateTime RoundDown(this DateTime dateTime, TimeSpan interval)
        {
            if (!dateTime.HasValue())
            {
                return dateTime;
            }

            var delta = dateTime.Ticks % interval.Ticks;
            return new DateTime(dateTime.Ticks - delta, dateTime.Kind);
        }

        private static DateTime RoundToNearest(this DateTime dateTime, TimeSpan interval)
        {
            var delta = dateTime.Ticks % interval.Ticks;
            var roundUp = delta > interval.Ticks / 2;

            return roundUp ? dateTime.RoundUp(interval) : dateTime.RoundDown(interval);
        }
    }
}