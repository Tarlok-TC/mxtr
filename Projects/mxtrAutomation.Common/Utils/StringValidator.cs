using System.Text.RegularExpressions;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Common.Utils
{
    public static class StringValidator
    {
        public static bool IsValidEmail(string email)
        {
            if (email.IsNullOrEmpty())
                return false;
            return Regex.IsMatch(email, @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
