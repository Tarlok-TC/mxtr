using System;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Utils;

namespace mxtrAutomation.Common.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Creates a trend from a collection of data based on the number of objects in the collection. It uses 
        /// the current range and the previous range to separate the datasets and create a Trend.
        /// </summary>
        /// <param name="range">The date range.</param>
        /// <param name="dateFilter">The date property used for filtering.</param>
        /// <param name="polarity">The direction considered as a positive change.</param>
        public static Trend TrendCount<T>(this IEnumerable<T> list, DateRangeBase range,
            Func<T, DateTime> dateFilter, Polarity polarity)
        {
            double currentCount = list.Where(x => range.Contains(dateFilter(x))).Count();
            double previousCount = list.Where(x => range.Previous.Contains(dateFilter(x))).Count();

            return new Trend(currentCount, previousCount, polarity);
        }

        /// <summary>
        /// Creates a trend from a collection of data based on the aggregation of a property. It uses
        /// two separate values within the object to calculate the trend.
        /// </summary>
        /// <param name="currentSumFilter">The property containing the current data.</param>
        /// <param name="previousSumFilter">The property containing the previous data.</param>
        /// <param name="polarity">The direction considered as a positive change.</param>
        public static Trend TrendSum<T>(this IEnumerable<T> list, Func<T, double> currentSumFilter,
            Func<T, double> previousSumFilter, Polarity polarity)
        {
            double currentSum = list.Sum(currentSumFilter);
            double previousSum = list.Sum(previousSumFilter);

            return new Trend(currentSum, previousSum, polarity);
        }

        /// <summary>
        /// Creates a trend from a collection of data based on a property. It uses the current range and the 
        /// previous range to separate the datasets and create a trend.
        /// </summary>
        /// <param name="range">The date range.</param>
        /// <param name="dateFilter">The date property used for filtering.</param>
        /// <param name="sumFilter">The main property used to create the Trend. This value will be sumed.</param>
        /// <param name="polarity">The direction considered as a positive change.</param>
        public static Trend TrendSum<T>(this IEnumerable<T> list, DateRangeBase range,
            Func<T, DateTime> dateFilter, Func<T, double> sumFilter, Polarity polarity)
        {
            double currentSum = list.Where(x => range.Contains(dateFilter(x))).Sum(sumFilter);
            double previousSum = list.Where(x => range.Previous.Contains(dateFilter(x))).Sum(sumFilter);

            return new Trend(currentSum, previousSum, polarity);
        }

        /// <summary>
        /// Creates a current sum from a collection of data based on a property.
        /// </summary>
        /// <param name="range">The date range.</param>
        /// <param name="dateFilter">The date property used for filtering.</param>
        /// <param name="sumFilter">The main property used to create the Trend. This value will be summed.</param>
        public static long CurrentSum<T>(this IEnumerable<T> list, DateRangeBase range,
            Func<T, DateTime> dateFilter, Func<T, double> sumFilter)
        {
            double currentSum = list.Where(x => range.Contains(dateFilter(x))).Sum(sumFilter);

            return (long)currentSum;
        }        
    }
}
