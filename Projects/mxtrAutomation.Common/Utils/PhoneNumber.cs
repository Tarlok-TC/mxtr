using System;

namespace mxtrAutomation.Common.Utils
{
    public class PhoneNumber : IPhoneNumber
    {
        /// <summary>
        /// The country calling code.
        /// </summary>
        public string CallingCode { get; set; }

        /// <summary>
        /// Numbering Plan Area Code. The first 3 numbers in a phone number.
        /// </summary>
        public string Npa { get; set; }

        /// <summary>
        /// Central Office Code (exchange). The 2nd set of 3 numbers in a phone number.
        /// </summary>
        public string Nxx { get; set; }

        /// <summary>
        /// Subscriber number. The last 4 digits.
        /// </summary>
        public string Xxxx { get; set; }

        /// <summary>
        /// The raw number input.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// The number with all non-digit characters removed.
        /// </summary>
        public string Sanitized { get; set; }

        /// <summary>
        /// The length of the phone number.
        /// </summary>
        public int Length { get; set; }

        public PhoneNumber(string number)
        {
            Sanitized = Sanitize(number);
            CallingCode = "1";

            if (!isValidLength(Sanitized))
            {
                throw new ArgumentException("Not a valid 7, 10, or 11 digit phone number.", "number");
            }

            Raw = number;
            Length = Sanitized.Length;

            int skip = 0;

            if (Length == 11)
            {
                CallingCode = Sanitized.Substring(skip, 1);
                skip += 1;
            }

            if (Length >= 10)
            {
                Npa = Sanitized.Substring(skip, 3);
                skip += 3;
            }

            Nxx = Sanitized.Substring(skip, 3);
            skip += 3;

            Xxxx = Sanitized.Substring(skip, 4);

        }

        public override string ToString()
        {
            if (Length == 11)
            {
                return String.Format("+{0} ({1}) {2}-{3}", CallingCode, Npa, Nxx, Xxxx);
            }
            else if (Length == 10)
            {
                return String.Format("({0}) {1}-{2}", Npa, Nxx, Xxxx);
            }
            else
            {
                return String.Format("{0}-{1}", Nxx, Xxxx);
            }
        }

        public static string Sanitize(string number)
        {
            return RegularExpressions.NotANumber.Replace(number, String.Empty);
        }

        public static bool IsValid(string number)
        {
            string sanitizedNumber = Sanitize(number);
            return isValidLength(sanitizedNumber);
        }

        private static bool isValidLength(string sanitizedNumber)
        {
            int length = sanitizedNumber.Length;

            if (length == 7 || length == 10 || length == 11)
            {
                return true;
            }

            return false;
        }
    }
}
