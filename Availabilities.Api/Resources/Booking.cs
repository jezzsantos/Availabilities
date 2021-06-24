using System;

namespace Availabilities.Resources
{
    public class Booking : IHasIdentifier
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
    }
}