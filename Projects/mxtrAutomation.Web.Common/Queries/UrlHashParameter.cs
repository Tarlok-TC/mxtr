using System;
using System.Text.RegularExpressions;

namespace mxtrAutomation.Web.Common.Queries
{
    public class UrlHashParameter : UrlParameterBase<string>
    {
        public UrlHashParameter(string propertyName, bool isRequired) : base(propertyName, isRequired) { }

        public UrlHashParameter(string propertyName) : base(propertyName) { }

        public UrlHashParameter(string propertyName, bool isRequired, Func<string, string> serializer, Func<string, string> deserializer)
            : base(propertyName, isRequired, serializer, deserializer) { }

        public override bool IsUrlPathParameter { get { return false; } }

        /// <summary>
        /// Formats the hash for output in the format "{0}".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //NOTE: Shouldn't this be UrlEncoded?
            return IsUsed && IsValid
                ? GetValue()
                : string.Empty;
        }

        public override string RegexMatchString
        {
            get
            {
                return string.Format(@"(\#)(([^\#]*?)|)({0}=(?<{0}>))", PropertyName);
            }
        }

        public override void SetValue(string rawInput)
        {
            Value = (Deserializer != null) ? Deserializer(rawInput) : rawInput;
        }

        public override void SetValue(Match m)
        {
            SetValue(m.Groups["Hash"].Value);
        }
    }
}
