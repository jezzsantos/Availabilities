﻿using System.Collections.Generic;
using Availabilities.Resources;

namespace Availabilities.Apis.ServiceOperations.Availabilities
{
    public class GetAllAvailabilitiesResponse
    {
        public List<Availability> Availabilities { get; set; }
    }
}