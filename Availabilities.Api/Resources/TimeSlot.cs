using System;

namespace Availabilities.Resources
{
    public class TimeSlot
    {
        private static readonly DateTime MinValue = DateTime.MinValue;
        private static readonly DateTime MaxValue = DateTime.MaxValue;
        private DateTime end;
        private DateTime start;

        public TimeSlot()
            : this(MinValue, MaxValue)
        {
        }

        public TimeSlot(DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
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

        public TimeSpan AbsoluteDuration => this.end.Subtract(this.start).Duration();
    }
}