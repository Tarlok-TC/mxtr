using System.Web.Mvc;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class TagBuilderExtensions
    {
        public static TagBuilder Attribute(this TagBuilder tag, string attributeName, string attributeValue)
        {
            if (!attributeValue.IsNullOrEmpty())
            {
                if (tag.Attributes.ContainsKey(attributeName))
                    tag.Attributes[attributeName] += " " + attributeValue;
                else
                    tag.MergeAttribute(attributeName, attributeValue);
            }

            return tag;
        }

        public static TagBuilder InnerHtml(this TagBuilder tag, string text)
        {
            if (!text.IsNullOrEmpty())
                tag.InnerHtml += text;

            return tag;
        }
    }
}
