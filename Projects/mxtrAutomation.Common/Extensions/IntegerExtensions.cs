using System;
using System.Text;

namespace mxtrAutomation.Common.Extensions
{
    public static class IntegerExtensions
    {
        public static string ToWords(this int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + Math.Abs(number).ToWords();

            StringBuilder words = new StringBuilder();

            if ((number / 1000000) > 0)
            {
                words.Append((number/1000000).ToWords() + " million ");
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words.Append((number / 1000).ToWords() + " thousand ");
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words.Append((number / 100).ToWords() + " hundred ");
                number %= 100;
            }

            if (number > 0)
            {
                if (words.Length > 0)
                    words.Append("and ");

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words.Append(unitsMap[number]);
                else
                {
                    words.Append(tensMap[number/10]);
                    if ((number % 10) > 0)
                        words.Append("-" + unitsMap[number%10]);
                }
            }

            return words.ToString();
        }
    }
}
