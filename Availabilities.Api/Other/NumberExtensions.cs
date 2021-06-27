namespace Availabilities.Other
{
    public static class NumberExtensions
    {
        public static bool IsDivisibleBy(this int value, int divisor)
        {
            return value % divisor == 0;
        }
    }
}