using System;

namespace mxtrAutomation.Common.Utils
{
    public static class EnumUtils
    {
        // I believe .NET 4 has an Enum.TryParse() method, but .NET 3.5 does not.
        public static T TryParseEnum<T>(string value)
        {

            if (!typeof(T).IsEnum)
                throw new InvalidOperationException("Attempt to parse enum on a non-enum type.");

            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return default(T);
            }
        }

    }
}
