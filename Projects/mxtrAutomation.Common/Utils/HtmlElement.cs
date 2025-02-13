using System.Collections.Generic;

namespace mxtrAutomation.Common.Utils
{
    public class HtmlElement
    {
        public string Html { get; set; }
        public string InnerHtml { get; set; }
        public string Selector { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
