using System.Text.RegularExpressions;

namespace mxtrAutomation.Web.Common.Queries
{
    public class EmailQueryStringParameter : RegexQueryStringParameter
    {
        private static readonly Regex EmailRegex
            = new Regex("^[_a-z0-9-]+(\\.[_a-z0-9-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*(\\.[a-z]{2,4})$");

        public EmailQueryStringParameter(string propertyName, bool isRequired)
            : base(EmailRegex, propertyName, isRequired)
        {
        }

        public EmailQueryStringParameter(string propertyName)
            : base(EmailRegex, propertyName)
        {
        }
    }
}
