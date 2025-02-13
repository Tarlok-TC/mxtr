using System.Web.Mvc;
using System.Xml.Linq;
using mxtrAutomation.Web.Common.Extensions;

namespace mxtrAutomation.Web.Common.Helpers
{
    public class AjaxLink : ViewLink
    {
        public AjaxLink(ViewLink viewLink)
        {
            Text = viewLink.Text;
            Url = viewLink.Url;
            Image = viewLink.Image;
            OnClick = viewLink.OnClick;
        }

        public AjaxLink() { }

        public override string ToString()
        {
            return
                new TagBuilder("span")
                    .Attribute("href", Url)
                    .Attribute("class", "ajaxMe")
                    .Attribute("postProcess", OnClick)
                    .InnerHtml(Text)
                    .ToString();
        }
    }
}
