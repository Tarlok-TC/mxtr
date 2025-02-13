using System.IO;
using System.Web.Mvc;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Web.Common.Extensions;

namespace mxtrAutomation.Web.Common.Helpers
{
    public class WebImage
    {
        public virtual string FileName { get; set; }
        public virtual string BasePath { get; set; }
        public virtual string AltText { get; set; }
        public virtual string Title { get; set; }
        public virtual string CssClass { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public virtual string Url
        {
            get { return Path.Combine(BasePath.Coalesce(s => s, string.Empty), FileName).Replace('\\', '/'); }
        }

        public override string ToString()
        {
            TagBuilder tag =
                new TagBuilder("img")
                    .Attribute("src", Url)
                    .Attribute("title", Title)
                    .Attribute("class", CssClass)
                    .Attribute("alt", AltText);

            if (Title.IsNullOrEmpty())
                tag.Attribute("title", AltText);

            return tag.ToString(TagRenderMode.SelfClosing);
        }

        public static implicit operator string(WebImage webImage)
        {
            return (webImage == null) ? null : webImage.ToString();
        }
    }
}
