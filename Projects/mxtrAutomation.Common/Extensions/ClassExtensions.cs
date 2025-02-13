using System;

namespace mxtrAutomation.Common.Extensions
{
    public static class ClassExtensions
    {
        public static OUT Coalesce<IN, OUT>(this IN instance, Func<IN, OUT> func, OUT defaultValue)
        {
            return (instance == null || instance.Equals(default(IN))) ? defaultValue : func(instance);
        }

        public static OUT Coalesce<IN, OUT>(this IN instance, Func<IN, OUT> func)
        {
            return (instance == null || instance.Equals(default(IN))) ? default(OUT) : func(instance);
        }

        public static T With<T>(this T obj, Action<T> act)
        {
            act(obj);
            return obj;
        }
    }
}
