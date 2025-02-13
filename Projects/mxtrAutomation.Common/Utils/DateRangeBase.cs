using System;

namespace mxtrAutomation.Common.Utils
{
    /// <summary>
    /// A range of two dates.
    /// </summary>
    /// <remarks>
    /// I chose to make this class inclusive/inclusive rather than inclusive/exclusive because
    /// of the way we handle dates on the front end. When we display a range (1/1 - 1/15 for example)
    /// what we really want is data from the 1st until the end of the 15th (11:59:59pm). If this were
    /// exclusive we'd have the end date be the 16th which would cause us to do conversions anytime 
    /// we wanted to display anything.
    /// 
    /// This also comes into play when working with two ranges together. With inclusive/inclusive
    /// we can safely make two ranges (1/1 - 1/15) and (1/16 - 1/30) and it have the correct meaning.
    /// With inclusive/exclusive we'd have to use (1/1 - 1/16) and (1/16 - 1/31) which requires conversion.
    /// </remarks>
    public abstract class DateRangeBase
    {
        private DateRangeBase _previous;

        public DateTime Start { get; internal set; }
        public DateTime End { get; internal set; }
        public DateRangeBase Previous
        {
            get
            {
                if (_previous == null)
                {
                    _previous = perviousRange();
                }

                return _previous;
            }
        }

        public DateRangeBase(DateTime start, DateTime end)
        {
            if (end < start)
            {
                throw new InvalidOperationException("Start date must be before end date.");
            }

            Start = start;
            End = end;
        }

        /// <summary>
        /// Determines if the range contains a date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool Contains(DateTime date)
        {
            return date >= Start && date <= End;
        }

        /// <summary>
        /// Determines if two time ranges have intersecting days.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool Intersect(DateRangeBase range)
        {
            return Start <= range.End && End >= range.Start;
        }

        public string ToShortDateString()
        {
            return Start.Date == End.Date
                ? Start.ToString("d")
                : String.Format("{0:d} - {1:d}", Start, End);
        }

        internal abstract DateRangeBase perviousRange();
    }
}
