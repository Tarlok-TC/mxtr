using System;

namespace mxtrAutomation.Common.Extensions
{
    public static class DateTimeExtensions
    {
        private static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToUtc(this DateTime dateTime, TimeZoneInfo sourceTimeZone)
        {
            DateTime dt = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(dt, sourceTimeZone);
        }

        public static DateTime ToTimeZoneFromUtc(this DateTime dateTime, TimeZoneInfo targetTimeZone)
        {
            DateTime dt = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeFromUtc(dt, targetTimeZone);
        }

        public static DateTime ToTimeZone(this DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo targetTimeZone)
        {
            DateTime dt = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTime(dt, sourceTimeZone, targetTimeZone);
        }

        public static DateTime ToCancelDate(this DateTime dateTime)
        {
            DateTime current = System.DateTime.Now;

            int anniversaryDay = dateTime.Day;

            if (current.Day < anniversaryDay)
            {
                return new DateTime(current.Year, current.Month, anniversaryDay);
            }
            else
            {
                DateTime thisMonthAnniversary = new DateTime(current.Year, current.Month, anniversaryDay);
                return thisMonthAnniversary.AddMonths(1);
            }
        }

        public static long ToUnixTimeStampMilliseconds(this DateTime dateTime)
        {
            long ll = 1;

            var xx = ll.ToDateTime();

            return (long)(DateTime.UtcNow - _epoch).TotalMilliseconds;
        }

        public static DateTime ToDateTime(this long unixTimeStampMilliseconds)
        {
            return _epoch.AddMilliseconds(unixTimeStampMilliseconds);
        }
    }
}
