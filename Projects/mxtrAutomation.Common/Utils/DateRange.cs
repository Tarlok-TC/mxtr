using System;
using System.Collections;
using System.Collections.Generic;

namespace mxtrAutomation.Common.Utils
{
    /// <summary>
    /// A range of two dates. Ignores time.
    /// </summary>
    public class DateRange : DateRangeBase, IEnumerable<DateTime>
    {
        public double Count
        {
            get
            {
                return (End - Start).TotalDays + 1;
            }
        }

        public DateRange(DateTime start, DateTime end)
            : base(start.Date, end.Date) { }

        /// <summary>
        /// Counts the number of intersecting days.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public double IntersectCount(DateRange range)
        {
            if (!Intersect(range))
            {
                return 0;
            }

            DateTime latestStart = Start > range.Start ? Start : range.Start;
            DateTime earliestEnd = End < range.End ? End : range.End;

            return (earliestEnd - latestStart).TotalDays + 1;
        }

        internal override DateRangeBase perviousRange()
        {
            TimeSpan span = End - Start;

            return new DateRange(Start.AddDays(-1) - span, Start.AddDays(-1));
        }

        public override string ToString()
        {
            return ToShortDateString();
        }

        #region IEnumerable Implementation
        public IEnumerator<DateTime> GetEnumerator()
        {
            for (DateTime s = Start.Date; s.Date <= End.Date; s = s.AddDays(1))
            {
                yield return s;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
