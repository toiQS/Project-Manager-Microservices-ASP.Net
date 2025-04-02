namespace Shared.Core.CommonUtils
{
    public static class Guard
    {
        public static void AgainstNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
        }

        public static void AgainstNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
        }

        public static void AgainstOutOfRange(int value, int min, int max, string paramName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
        }
    }
}
