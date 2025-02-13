using System;
using System.Collections.Generic;

namespace mxtrAutomation.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static U GetValueOrDefault<T, U>(this Dictionary<T, U> dict, T key)
        {
            U result;
            return dict.TryGetValue(key, out result) ? result : default(U);
        }

        public static U GetValueOrDefault<T, U>(this Dictionary<T, U> dict, T key, U defaultValue)
        {
            U result;
            return dict.TryGetValue(key, out result) ? result : defaultValue;
        }

        public static U GetValueOrDefault<T, U>(this Dictionary<T, U> dict, T key, Func<U> valueProvider)
        {
            U result;
            return dict.TryGetValue(key, out result) ? result : valueProvider();
        }
    }
}
