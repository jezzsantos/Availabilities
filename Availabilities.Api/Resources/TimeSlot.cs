using System;

namespace Availabilities.Resources
{
    public class TimeSlot
    {
        private static readonly DateTime MinValue = DateTime.MinValue;
        private static readonly DateTime MaxValue = DateTime.MaxValue;
        private DateTime end;
        private DateTime start;

        public TimeSlot(DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
        }

        public TimeSlot(DateTime start, int durationInMinutes) : this(start, TimeSpan.FromMinutes(durationInMinutes))
        {
        }

        public TimeSlot(DateTime start, TimeSpan duration)
        {
            this.start = start;
            this.end = start.Add(duration);
        }

        public DateTime Start
        {
            get => this.start;
            set
            {
                if (value > this.end)
                {
                    this.start = value;
                    this.end = MaxValue;
                }

                this.start = value;
            }
        }

        public DateTime End
        {
            get => this.end;
            set
            {
                if (value < this.start)
                {
                    this.start = MinValue;
                    this.end = value;
                }

                this.end = value;
            }
        }

        public TimeSpan Duration => this.end.Subtract(this.start);
    }
}