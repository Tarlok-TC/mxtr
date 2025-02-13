using System.Text.RegularExpressions;

namespace mxtrAutomation.Websites.Platform.Helpers
{
    public static class AccountHelpers
    {
        public static string CreateDomainName(string name)
        {
            return Regex.Replace(name, @"[^0-9a-zA-Z]+", "-").ToLower();
        }
    }
}