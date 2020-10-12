using System;

namespace CMS.Common.DateToolkit
{
    public static class DateExtensions
    {
        public static long ToUnixTimeStamp(this DateTime date)
        {
            var unixTimestamp = date.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }

        public static long? ToUnixTimeStamp(this DateTime? date)
        {
            if (date == null)
                return null;
            var unixTimestamp = date.Value.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }

        public static DateTime ToDateTimeFromUnix(this long unixTime)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime? ToDateTimeFromUnix(this long? unixTime)
        {
            if (unixTime == null)
                return null;
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTime.Value).ToLocalTime();
            return dtDateTime;
        }
    }
}
