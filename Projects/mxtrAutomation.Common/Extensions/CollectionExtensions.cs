using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Items;

namespace mxtrAutomation.Common.Extensions
{
    public static class CollectionExtensions
    {
#if false
        public static bool IsNullOrEmpty(this ICollection collection)
        {
            return (collection == null || collection.Count == 0);
        }

        public static int CountOrZeroIfNull(this ICollection collection)
        {
            return (collection == null) ? 0 : collection.Count;
        }
#endif

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return (collection == null || collection.Count() == 0);
        }

        public static int CountOrZeroIfNull<T>(this IEnumerable<T> collection)
        {
            return (collection == null) ? 0 : collection.Count();
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> func)
        {
            foreach (T item in source)
                func(item);
        }

        public static void ForEach<T>(this IEnumerable source, Action<T> func)
            where T : class
        {
            source.OfType<T>().ForEach(func);
        }

        public static IEnumerable<EndedItem<T>> ToEnded<T>(this IEnumerable<T> source)
        {
            int count = source.Count();

            return source.Select((item, idx) => new EndedItem<T>
                                                    {
                                                        Item = item,
                                                        IsFirst = (idx == 0),
                                                        IsLast = (idx == count - 1),
                                                        Index = idx
                                                    });
        }

        public static string ToString(this IEnumerable source, char joinChar)
        {
            return source.OfType<object>().ToString(joinChar);
        }

        public static string ToString(this IEnumerable source, string joinStr)
        {
            return source.OfType<object>().ToString(joinStr);
        }

        public static string ToString<T>(this IEnumerable<T> source, char joinChar)
        {
            return source.ToString(joinChar.ToString());
        }

        public static string ToString<T>(this IEnumerable<T> source, string joinStr)
        {
            if (source == null)
                return string.Empty;

            return string.Join(joinStr, source.Select(s => s.ToString()).ToArray());
        }

        public static bool Exists<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Any(predicate);
        }

        // By being public, we can manually seed...  Useful for creating consistant random seed data,
        // or for unit testing.
        public static Random _random = new Random();

        public static T Random<T>(this IEnumerable<T> list)
        {
            if (list.IsNullOrEmpty())
                throw new InvalidOperationException("Cannot select a random element because collection is null or empty.");

            return list.ElementAt(_random.Next(list.Count()));
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> list)
        {
            if (list == null)
                throw new InvalidOperationException("Cannot randomize collection because the collection is null.");

            return list.Select(x => new { Item = x, Order = _random.Next() })
                       .OrderBy(x => x.Order)
                       .Select(x => x.Item);
        }

        public static T ElementAt<T>(this IQueryable<T> list, int index)
        {
            //return list.Skip(index).FirstOrDefault();
            return list.Skip(index).Take(1).FirstOrDefault();
        }

        public static T? GetSingleScalar<T>(this IQueryable<T> list)
            where T : struct 
        {
            return list.Any() ? list.SingleOrDefault() : (T?) null;
        }

        public static IEnumerable<T> Random<T>(this IQueryable<T> list, int take)
        {
            return RandomIndexes(0, list.Count(), take).Select(index => list.ElementAt(index));
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> list, int take)
        {
            return RandomIndexes(0, list.Count(), take).Select(index => list.ElementAt(index));
        }

        public static IEnumerable<int> RandomIndexes(int start, int count, int take)
        {
            if (take > count / 4)
                return Enumerable.Range(start, count).Randomize().Take(take);

            HashSet<int> result = new HashSet<int>();
            while (result.Count() < take)
                result.Add(_random.Next(start, count));

            return result;
        }

        public static IEnumerable<TResult> Zip<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, TResult> zipper)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            if (zipper == null)
                throw new ArgumentNullException("zipper");

            using (var leftEnum = left.GetEnumerator())
            using (var rightEnum = right.GetEnumerator())
            {
                while (leftEnum.MoveNext() && rightEnum.MoveNext())
                {
                    yield return zipper(leftEnum.Current, rightEnum.Current);
                }
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (items == null)
                throw new ArgumentNullException("items");
            items.ForEach(collection.Add);
            return collection;
        }

        public static bool HasDuplicate<T>(this IEnumerable<T> source)
        {
            return source.GroupBy(x => x).Any(g => g.Count() > 1);
        }

        public static bool HasDuplicate<T,U>(this IEnumerable<T> source, Func<T,U> func)
        {
            return source.Select(func).HasDuplicate();
        }

        public static bool Try<T>(this IEnumerable<T> source, Action<T> func)
        {
            return source.All(s => s.Try(func));
        }

        public static bool Try<T>(this T source, Action<T> func)
        {
            try
            {
                func(source);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    yield return ChunkSequence(enumerator, chunkSize).ToList();
            }
        }

        private static IEnumerable<T> ChunkSequence<T>(IEnumerator<T> enumerator, int chunkSize)
        {
            int count = 0;

            do
                yield return enumerator.Current;
            while (++count < chunkSize && enumerator.MoveNext());
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (T element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
