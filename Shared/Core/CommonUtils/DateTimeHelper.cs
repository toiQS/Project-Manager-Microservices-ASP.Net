namespace Shared.Core.CommonUtils
{
    public static class DateTimeHelper
    {
        public static DateTime NowUtc() => DateTime.UtcNow;

        public static int GetDaysDifference(DateTime start, DateTime end)
        {
            return (end - start).Days;
        }

        public static string ToIsoString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
    }

}
