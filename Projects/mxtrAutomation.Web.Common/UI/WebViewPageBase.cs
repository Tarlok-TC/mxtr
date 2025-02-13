using System.Web.Mvc;

namespace mxtrAutomation.Web.Common.UI
{
    public abstract class WebViewPageBase<T> : WebViewPage<T>
    {
        public override void Write(object value)
        {
            base.WriteLiteral(value);
        }
    }

    public abstract class WebViewPageBase : WebViewPage
    {
        public override void Write(object value)
        {
            base.WriteLiteral(value);
        }
    }
}