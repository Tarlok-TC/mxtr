using System;
using System.Collections.Generic;
using System.Linq;

namespace mxtrAutomation.Common.Extensions
{
    public static class PagingExtensions
    {
        /// <summary>
        /// Grabs a page within a collection of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="page">The current page (0 based).</param>
        /// <returns>A collection of elements representing the current page.</returns>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> items, int pageSize, int page)
        {
            return items.Skip(page * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Calculates the total number of pages in a collection for the given page size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="pageSize">The size of each page.</param>
        /// <returns>How many pages of data are in the colleciton.</returns>
        public static int TotalPages<T>(this IEnumerable<T> items, int pageSize)
        {
            if (pageSize < 1)
            {
                throw new ArgumentException("Value must be greater than 0.", "pageSize");
            }

            if (!items.Any())
            {
                return 0;
            }

            return (int)Math.Ceiling((double)items.Count() / (double)pageSize);
        }
    }
}
