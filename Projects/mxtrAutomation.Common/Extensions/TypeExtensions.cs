using System;

namespace mxtrAutomation.Common.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static bool IsNullable<T>(this T obj)
            where T : class
        {
            if (obj == null)
                return true; // obvious

            Type type = typeof(T);

            if (!type.IsValueType)
                return true; // ref-type is nullable

            if (Nullable.GetUnderlyingType(type) != null)
                return true; // Nullable<T>

            return false; // value-type
        }

    }
}
