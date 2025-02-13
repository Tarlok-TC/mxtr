using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Web.Common.Queries
{
    public static class SerializationHelpers
    {
        public static readonly string PatternGuid = string.Intern(@"(([\dA-Fa-f]{8})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{12}))|(\{([\dA-Fa-f]{8})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{12})\})");

        #region yes|no - bool

        public static string SerializeBoolToYesNo(bool input)
        {
            return input ? "yes" : "no";
        }

        public static string SerializeBoolToCasedYesNo(bool input)
        {
            return input ? "Yes" : "No";
        }

        public static bool DeserializeBoolFromYesNo(string input)
        {
            string yesNoValue = input.ToLower();

            if (yesNoValue == "y" || yesNoValue == "yes")
                return true;

            if (yesNoValue == "n" || yesNoValue == "no")
                return false;

            throw new ArgumentOutOfRangeException("Values must be \"yes\" or \"no\"");
        }

        public static string SerializeNullableBoolToYesNo(bool? input)
        {
            return input == null ? "no" : input.Value ? "yes" : "no";
        }

        public static bool? DeserializeNullableBoolFromYesNo(string input)
        {
            string yesNoValue = input.ToLower();
            if (yesNoValue == "y" || yesNoValue == "yes")
                return true;
            else if (yesNoValue == "n" || yesNoValue == "no")
                return false;
            throw new ArgumentOutOfRangeException("Values must be \"yes\" or \"no\"");
        }

        public static int SerializeBoolToInteger(bool input)
        {
            return (input ? 1 : 0);
        }

        public static bool DeserializeBoolFromInteger(int input)
        {
            return input != 0;
        }

        #endregion

        #region Guid - string

        private static readonly Regex RegexGuid = new Regex("^" + PatternGuid.Replace("|", "$|^") + "$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public static string SerializeGuidToString(Guid input)
        {
            if (input.Equals(Guid.Empty))
                return string.Empty;
            return input.ToString("D");
        }


        public static Guid DeserializeGuidFromString(string input)
        {
            if (RegexGuid.IsMatch(input))
            {
                return new Guid(input);
            }
            return Guid.Empty;
        }

        #endregion

        #region List

        public static ICollection<T> DeserializeListFromString<T>(string rawInput, Func<string,T> deserializer)
        {
            return DeserializeListFromStringList(rawInput.Coalesce(x => x.Split(',')), deserializer);
        }

        public static ICollection<T> DeserializeListFromStringList<T>(IEnumerable<string> input, Func<string,T> deserializer)
        {
            if (input == null)
                return new List<T>();

            return
                input.Where(v => v != null).Select(v => v.Trim()).Where(v => v.Length > 0).Select(deserializer).ToList();
        }

        public static string SerialzeListToString<T>(IEnumerable<T> values, Func<T,string> serializer)
        {
            return values.Coalesce(x => x.Where(v => v != null).Select(serializer).ToString(","), string.Empty);
        }

        #endregion

        #region String List

        public static ICollection<string> DeserializeStringListFromString(string rawInput)
        {
            List<string> list = new List<string>();

            if (!string.IsNullOrEmpty(rawInput))
                list.AddRange(rawInput.Split(',').Select(value => value.Trim()).Where(trimValue => trimValue.Length > 0));

            return list;
        }

        public static ICollection<string> DeserializeStringListFromStringList(ICollection<string> input)
        {
            if (input == null)
                return new List<string>();

            return input.Where(v => v != null).Select(v => v.Trim()).Where(v => v.Length > 0).ToList();
        }

        public static string SerialzeStringListToString(ICollection<string> values)
        {
            StringBuilder sb = new StringBuilder();

            if (values != null && values.Count > 0)
            {
                foreach (var trimValue in values.Where(value => !string.IsNullOrEmpty(value)).Select(value => value.Trim()).Where(trimValue => trimValue.Length > 0))
                {
                    if (sb.Length > 0)
                        sb.Append(',');
                    sb.Append(trimValue);
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Version - string

        public static string SerializeVersionToString(Version input)
        {
            return input.ToString();
        }

        public static Version DeserializeVersionFromString(string input)
        {
            try
            {
                return new Version(input);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Short date

        public static string SerializeDateToString(DateTime input)
        {
            return input.ToString("MMddyyyy", CultureInfo.InvariantCulture);
        }

        public static DateTime DeserializeDateFromString(string input)
        {
            try
            {
                return DateTime.ParseExact(input, "MMddyyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                return default(DateTime);
            }
        }

        #endregion
    }
}
