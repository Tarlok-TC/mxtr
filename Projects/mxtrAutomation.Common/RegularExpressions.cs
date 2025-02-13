using System.Text.RegularExpressions;

namespace mxtrAutomation.Common
{
    public static class RegularExpressions
    {
        public static readonly string PatternGuid = string.Intern(@"(([\dA-Fa-f]{8})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{12}))|(\{([\dA-Fa-f]{8})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{4})\-([\dA-Fa-f]{12})\})");
        public static readonly string PatternBase64 = string.Intern(@"([a-zA-Z0-9\+\/]*)\=*");
        public static readonly string PatternBase64Variant = string.Intern(@"([a-zA-Z0-9\-_]*)");
        public static readonly string PatternNotANumber = string.Intern(@"[^0-9]");
        public static readonly string PatternThreeDigits = string.Intern(@"[0-9]{3}");
        public static readonly string PatternWhiteSpace = string.Intern(@"\s+");

        public static readonly Regex ExpressionGuid = new Regex(PatternGuid, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        public static readonly Regex ExpressionBase64 = new Regex(PatternBase64, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        public static readonly Regex ExpressionBase64Variant = new Regex(PatternBase64Variant, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        public static readonly Regex NotANumber = new Regex(PatternNotANumber, RegexOptions.Compiled);
        public static readonly Regex ThreeDigits = new Regex(PatternThreeDigits, RegexOptions.Compiled);
        public static readonly Regex WhiteSpace = new Regex(PatternWhiteSpace, RegexOptions.Compiled);

    }
}
