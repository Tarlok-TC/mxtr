using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using mxtrAutomation.Common.Utils;

namespace mxtrAutomation.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Condenses multiple spaces into a single space.
        /// </summary>
        public static string CollapseWhitespace(this string value)
        {
            return RegularExpressions.WhiteSpace.Replace(value, " ");
        }

        public static T GetEnumValue<T>(this string value) where T:struct, IConvertible
        {
            if (typeof(T).IsEnum)
            {
                List<T> tmpEnumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();

                T tmpSelectedEnumValue = tmpEnumValues.Where(item => (item as Enum) != null &&
                        (item as Enum).GetDescription().Equals(value)).FirstOrDefault();

                return tmpSelectedEnumValue;
            }
            else
            {
                throw new ArgumentException("T must be an enumerated type");
            }
        }

        /// <summary>
        /// Converts a string to title case format for the current culture
        /// </summary>
        public static string ToTitleCurrentCulture(this string value)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        /// <summary>
        /// Truncates a string to a specified length. If the string is 
        /// shorter than the maximum length, the original string is returned.
        /// </summary>
        /// <param name="maxLength">The max string length.</param>
        public static string Truncate(this string value, int maxLength)
        {
            if (maxLength < 0)
            {
                throw new ArgumentException("Must be a positive value.", "maxLength");
            }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }

        public static string[] EscapedSplit(this string value, string separator)
        {
            return value.EscapedSplit(separator, StringComparison.CurrentCulture);
        }

        public static string[] EscapedSplit(this string value, string separator, StringComparison comparison)
        {
            List<string> result = new List<string>();
            string escape = "\\";
            string entry = String.Empty;

            int previousIndex = 0;
            int index = value.IndexOf(separator, previousIndex, comparison);

            while (index > -1)
            {
                int escapeIndex = index - escape.Length;
                string splitEscape = value.Substring(escapeIndex, escape.Length);
                bool escaped = String.Equals(escape, splitEscape, comparison);

                if (!escaped)
                {
                    entry += value.Substring(previousIndex, index - previousIndex);
                    result.Add(entry);

                    entry = String.Empty;
                }
                else
                {
                    entry += value.Substring(previousIndex, escapeIndex - previousIndex);
                    entry += value.Substring(index, separator.Length);
                }


                previousIndex = index + separator.Length;
                index = value.IndexOf(separator, index + separator.Length, comparison);
            }

            entry += value.Substring(previousIndex, value.Length - previousIndex);
            result.Add(entry);

            return result.ToArray();
        }

        public static string ToProperCase(this string str)
        {
            return str.Split(' ').Select(StringUtils.ConvertWordToProperCase).ToString(' ');
        }

        public static string ToPossessive(this string str)
        {
            return StringUtils.ConvertWordToPossessive(str);
        }

        public static string UnitsOf(this int count, string unit)
        {
            return count.ToString("#,##0") + " " + StringUtils.Pluralize(count, unit);
        }

        public static string Teaser(this string str, int length)
        {
            return StringUtils.Teaser(str, length);
        }

        public static string StripHtml(this string str)
        {
            return StringUtils.StripHtml(str);
        }

        public static T? TryParseEnum<T>(this string value) where T : struct
        {
            Type resultType = typeof(T);
            if (!resultType.IsEnum)
                throw new ArgumentException("The suppliedType must be an Enum");
            try
            {
                return (T)Enum.Parse(resultType, value, true);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static string With(this string input, params object[] parameters)
        {
            return string.Format(input, parameters);
        }

        public static string ExceptBlanks(this string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        continue;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
