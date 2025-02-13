
namespace mxtrAutomation.Common.Utils
{
    public interface IPhoneNumber
    {
        /// <summary>
        /// The country calling code.
        /// </summary>
        string CallingCode { get; }

        /// <summary>
        /// Numbering Plan Area Code. The first 3 numbers in a phone number.
        /// </summary>
        string Npa { get; }

        /// <summary>
        /// Central Office Code (exchange). The 2nd set of 3 numbers in a phone number.
        /// </summary>
        string Nxx { get; }

        /// <summary>
        /// Subscriber number. The last 4 digits.
        /// </summary>
        string Xxxx { get; }

        /// <summary>
        /// The raw number input.
        /// </summary>
        string Raw { get; }

        /// <summary>
        /// The number with all non-digit characters removed.
        /// </summary>
        string Sanitized { get; }

        /// <summary>
        /// The length of the phone number.
        /// </summary>
        int Length { get; }
    }
}
