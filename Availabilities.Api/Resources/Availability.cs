using System;

namespace Availabilities.Resources
{
    public class Availability : IHasIdentifier
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string Id { get; set; }
    }
}