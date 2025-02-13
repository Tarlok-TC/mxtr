using System;
using System.Collections.Generic;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Common.Utils
{
    /// <summary>
    /// A range of two dates and times.
    /// </summary>
    public class DateTimeRange : DateRangeBase
    {
        public TimeZoneInfo TimeZone { get; set; }

        public DateTimeRange(DateTime start, DateTime end)
            : this(start, end, (TimeZoneInfo)null) { }

        public DateTimeRange(DateTime start, DateTime end, string timeZoneId)
            : this(start, end, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)) { }

        public DateTimeRange(DateTime start, DateTime end, TimeZoneInfo timeZone)
            : base(setKind(start, timeZone), setKind(end, timeZone))
        {
            TimeZone = timeZone;
        }

        private static DateTime setKind(DateTime dateTime, TimeZoneInfo timeZone)
        {
            DateTimeKind kind = DateTimeKind.Unspecified;

            if (timeZone == TimeZoneInfo.Utc)
            {
                kind = DateTimeKind.Utc;
            }
            else if (timeZone == TimeZoneInfo.Local)
            {
                kind = DateTimeKind.Local;
            }

            return DateTime.SpecifyKind(dateTime, kind);
        }

        /// <summary>
        /// Creates a new time spanning from the beginning of the previous range to the end of
        /// the current range.
        /// </summary>
        /// <example>
        /// Current: 3/16 - 3/30
        /// Previous: 3/1 - 3/15
        /// Result: 3/1 - 3/30
        /// </example>
        /// <returns></returns>
        public DateTimeRange ToTrendRange()
        {
            return new DateTimeRange(Previous.Start, End, TimeZone);
        }

        /// <summary>
        /// Converts the range from it's current timezone to UTC.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when a timezone isn't specified</exception>
        /// <returns></returns>
        public DateTimeRange ToUtc()
        {
            if (TimeZone == null) throw new InvalidOperationException("TimeZone must be specified.");
            
            return new DateTimeRange(Start.ToUtc(TimeZone), End.ToUtc(TimeZone), TimeZoneInfo.Utc);
        }

        public DateTimeRange ToPrevious()
        {
            TimeSpan rangeSpan = End - Start;

            return new DateTimeRange(Start.AddSeconds(-1) - rangeSpan, Start.AddSeconds(-1), TimeZone);
        }

        /// <summary>
        /// Splits the time range into smaller segments. If the segment isn't evenly split the last range
        /// will contain the remainder.
        /// </summary>
        /// <param name="span">The length of the segments.</param>
        /// <returns>A collection of smaller segments.</returns>
        public IEnumerable<DateTimeRange> Split(TimeSpan span)
        {
            DateTime position = Start;
            while (position < End)
            {
                DateTime newPosition = position.Add(span);
                if (newPosition >= End)
                {
                    newPosition = End;
                }
                else
                {
                    newPosition = newPosition.AddTicks(-1);
                }

                yield return new DateTimeRange(position, newPosition);

                position = position.Add(span);
            }
        }

        public override string ToString()
        {
            if (TimeZone != null && TimeZone != TimeZoneInfo.Utc)
            {
                DateTimeOffset offsetStart = new DateTimeOffset(Start, TimeZone.BaseUtcOffset);
                DateTimeOffset offsetEnd = new DateTimeOffset(End, TimeZone.BaseUtcOffset);

                return Start == End
                    ? offsetStart.ToString("o")
                    : String.Format("{0:o} - {1:o}", offsetStart, offsetEnd);
            }
            else
            {
                return Start == End
                    ? Start.ToString("o")
                    : String.Format("{0:o} - {1:o}", Start, End);
            }
        }

        internal override DateRangeBase perviousRange()
        {
            return ToPrevious();
        }

        /// <summary>
        /// Creates new DateTimeRange with inclusive time ranges. Start date's time will be 12:00:00am 
        /// and the end date's time will be 11:59:59pm.
        /// </summary>
        public static DateTimeRange CreateInclusive(DateTime start, DateTime end)
        {
            return CreateInclusive(start, end, (TimeZoneInfo)null);
        }
        /// <summary>
        /// Creates new DateTimeRange with inclusive time ranges. Start date's time will be 12:00:00am 
        /// and the end date's time will be 11:59:59pm.
        /// </summary>
        public static DateTimeRange CreateInclusive(DateTime start, DateTime end, string timeZoneId)
        {
            if(string.IsNullOrEmpty(timeZoneId))
                timeZoneId = "Eastern Standard Time";

            return CreateInclusive(start, end, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }
        /// <summary>
        /// Creates new DateTimeRange with inclusive time ranges. Start date's time will be 12:00:00am 
        /// and the end date's time will be 11:59:59pm.
        /// </summary>
        public static DateTimeRange CreateInclusive(DateTime start, DateTime end, TimeZoneInfo timezone)
        {
            int hoursOffset = 0;
            
            if (timezone != null)
                hoursOffset = Math.Abs(timezone.BaseUtcOffset.Hours);

            DateTime newStart = start.AddHours(hoursOffset);
            DateTime newEnd = end.AddHours(hoursOffset);

            return new DateTimeRange(newStart, newEnd, timezone);
        }

        public static implicit operator DateRange(DateTimeRange range)
        {
            return new DateRange(range.Start, range.End);
        }
    }
}
