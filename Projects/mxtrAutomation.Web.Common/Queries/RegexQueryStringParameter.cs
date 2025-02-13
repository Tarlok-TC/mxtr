using System.Text.RegularExpressions;

namespace mxtrAutomation.Web.Common.Queries
{
    public class RegexQueryStringParameter : QueryStringParameter<string>
    {
        private readonly Regex _regex;

        public override bool IsValid
        {
            get { return _regex.IsMatch(Value) && base.IsValid; }
        }

        public RegexQueryStringParameter(Regex regex, string propertyName, bool isRequired)
            : base(propertyName, isRequired)
        {
            _regex = regex;
        }

        public RegexQueryStringParameter(Regex regex, string propertyName)
            : base(propertyName)
        {
            _regex = regex;
        }
    }
}
