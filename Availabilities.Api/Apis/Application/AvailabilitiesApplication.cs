using System.Linq;
using Availabilities.Other;
using Availabilities.Resources;
using Availabilities.Storage;
using ServiceStack;

namespace Availabilities.Apis.Application
{
    internal class AvailabilitiesApplication : IAvailabilitiesApplication
    {
        private readonly IStorage<Availability> storage;

        public AvailabilitiesApplication(IStorage<Availability> storage)
        {
            this.storage = storage;
        }

        public TimeSlot ReserveAvailability(TimeSlot slot)
        {
            var availableSlot = new TimeSlot(slot.Start.ToNextOrCurrentQuarterHour(),
                slot.Duration);

            var slots = this.storage.List();
            var availabilityToChange = slots.FirstOrDefault(s => s.ToSlot().EclipsesOrSameAs(availableSlot));
            if (availabilityToChange.NotExists())
            {
                throw new ResourceConflictException("There is no availability");
            }

            if (availabilityToChange.ToSlot().IsSameAs(availableSlot))
            {
                this.storage.Delete(availabilityToChange.Id);
            }
            else
            {
                if (availableSlot.Start == availabilityToChange.StartUtc)
                {
                    availabilityToChange.StartUtc = availableSlot.End;
                    this.storage.Upsert(availabilityToChange);
                }
                else if (availableSlot.End == availabilityToChange.EndUtc)
                {
                    availabilityToChange.EndUtc = availableSlot.Start;
                    this.storage.Upsert(availabilityToChange);
                }
                else if (availableSlot.Start > availabilityToChange.StartUtc)
                {
                    this.storage.Upsert(new Availability
                    {
                        StartUtc = availableSlot.End,
                        EndUtc = availabilityToChange.EndUtc
                    });
                    availabilityToChange.EndUtc = availableSlot.Start;
                    this.storage.Upsert(availabilityToChange);
                }
            }

            return availableSlot;
        }

        public void ReleaseAvailability(TimeSlot slot)
        {
            var slots = this.storage.List();

            //option: does it overlap that end of an existing slot?
            var availabilityToLengthenRight = slots.FirstOrDefault(s => s.ToSlot().OverlapsEndOf(slot));
            if (availabilityToLengthenRight.Exists())
            {
                // lengthen the availabilityToLengthenRight 
                availabilityToLengthenRight.EndUtc = slot.End;
                this.storage.Upsert(availabilityToLengthenRight);

                // reprocess: delete any existing slots that are now entirely overlapped by our availabilityToLengthenRight
                slots = this.storage.List();
                slots
                    .Where(s => s.ToSlot().EclipsedBy(availabilityToLengthenRight.ToSlot()))
                    .Where(availabilityOverlapped => availabilityOverlapped.Id != availabilityToLengthenRight.Id)
                    .Each(availabilityToDelete => { this.storage.Delete(availabilityToDelete.Id); });

                // reprocess: lengthen by any existing slots that are now partially overlapped by our availabilityToLengthenRight, and then delete those overlapped ones
                slots = this.storage.List();
                slots
                    .Where(s => s.ToSlot().OverlapsStartOf(availabilityToLengthenRight.ToSlot()))
                    .Where(availabilityOverlapped => availabilityOverlapped.Id != availabilityToLengthenRight.Id)
                    .Each(availabilityToDelete =>
                    {
                        availabilityToLengthenRight.EndUtc = availabilityToDelete.EndUtc;
                        this.storage.Upsert(availabilityToLengthenRight);
                        this.storage.Delete(availabilityToDelete.Id);
                    });

                return;
            }

            //option: does it overlap the start of an existing slot?
            var availabilityToLengthenLeft = slots.FirstOrDefault(s => s.ToSlot().OverlapsStartOf(slot));
            if (availabilityToLengthenLeft.Exists())
            {
                // lengthen the availabilityToLengthenLeft
                availabilityToLengthenLeft.StartUtc = slot.Start;
                this.storage.Upsert(availabilityToLengthenLeft);
            }
        }
    }

    public static class AvailabilityExtensions
    {
        public static TimeSlot ToSlot(this Availability availability)
        {
            return new TimeSlot(availability.StartUtc, availability.EndUtc);
        }

        public static bool IsSameAs(this TimeSlot target, TimeSlot source)
        {
            return source.IsValidSlot()
                   && source.Start == target.Start && source.End == target.End;
        }

        public static bool EclipsesOrSameAs(this TimeSlot target, TimeSlot source)
        {
            return source.IsValidSlot()
                   && source.Start >= target.Start && source.End <= target.End;
        }

        public static bool EclipsedBy(this TimeSlot target, TimeSlot source)
        {
            return source.IsValidSlot()
                   && source.Start <= target.Start && source.End >= target.End;
        }

        public static bool OverlapsStartOf(this TimeSlot target, TimeSlot source)
        {
            return source.IsValidSlot()
                   && source.End >= target.Start && source.End < target.End;
        }

        public static bool OverlapsEndOf(this TimeSlot target, TimeSlot source)
        {
            return source.IsValidSlot()
                   && source.Start <= target.End && source.End >= target.End;
        }

        private static bool IsValidSlot(this TimeSlot source)
        {
            return source.End > source.Start;
        }
    }
}