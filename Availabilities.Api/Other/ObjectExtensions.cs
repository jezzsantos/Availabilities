using JetBrains.Annotations;

namespace Availabilities.Other
{
    public static class ObjectExtensions
    {
        [ContractAnnotation("null => false; notnull => true")]
        public static bool Exists(this object value)
        {
            return value != null;
        }

        [ContractAnnotation("null => true; notnull => false")]
        public static bool NotExists(this object value)
        {
            return !value.Exists();
        }
    }
}