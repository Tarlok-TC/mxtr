using System.Web.Mvc;

namespace mxtrAutomation.Web.Common.ActionFilters
{
    public class CustomContentTypeAttribute : ActionFilterAttribute
    {
        public string ContentType { get; set; }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.ContentType = ContentType;
        }
    }
}
