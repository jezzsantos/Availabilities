﻿using System;
using System.Runtime.Serialization;

namespace Availabilities.Other
{
    [Serializable]
    public class ResourceConflictException : Exception
    {
        public ResourceConflictException()
            : base("The resource had a conflict")
        {
        }

        public ResourceConflictException(string message)
            : base(message)
        {
        }

        public ResourceConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ResourceConflictException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}