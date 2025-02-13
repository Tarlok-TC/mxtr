using System;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Web.Common.Extensions;

namespace mxtrAutomation.Web.Common.Queries
{
    public static class RouteValueKind
    {
        public static readonly string Controller = "controller";
        public static readonly string Action = "action";
        public static readonly string Query = "query";
    }

    public static class RouteTokenKind
    {
        public static readonly string Query = "query";
    }

    public abstract class WebQueryRouteBase : Route
    {
        public QueryBase Query { get; set; }

        protected WebQueryRouteBase() : base("", new MvcRouteHandler()) { }
    }

    public class WebQueryRoute<QUERY, CONTROLLER> : WebQueryRouteBase
        where QUERY : QueryBase, new()
        where CONTROLLER : Controller
    {
        private readonly string _controller;
        private readonly string _action;

        public WebQueryRoute(Expression<Func<CONTROLLER, Func<QUERY, ActionResult>>> actionExpression)           
        {
            _controller = GetControllerName(typeof (CONTROLLER));
            _action = GetActionName(actionExpression);

            Query = new QUERY();

            Url = Query.Route().Coalesce(x => x.Substring(1));
        }

        public WebQueryRoute(Expression<Func<CONTROLLER, Func<QueryBase, ActionResult>>> actionExpression)            
        {
            _controller = GetControllerName(typeof(CONTROLLER));
            _action = GetActionName(actionExpression);

            Query = new QUERY();

            Url = Query.Route().Coalesce(x => x.Substring(1));
        }

        protected string GetControllerName(Type controllerType)
        {
            string controllerName = controllerType.Name;

            return controllerName.Substring(0, controllerName.IndexOf("Controller"));
        }

        protected string GetActionName(Expression<Func<CONTROLLER, Func<QUERY, ActionResult>>> actionExpression)
        {
            return GetActionName(actionExpression as LambdaExpression);
        }
        
        protected string GetActionName(Expression<Func<CONTROLLER, Func<QueryBase, ActionResult>>> actionExpression)
        {
            return GetActionName(actionExpression as LambdaExpression);
        }

        protected string GetActionName(LambdaExpression actionExpression)
        {
            UnaryExpression convert = actionExpression.Body as UnaryExpression;

            if (convert == null)
                throw new InvalidOperationException("Invalid action expression.");

            MethodCallExpression methodCall = convert.Operand as MethodCallExpression;

            if (methodCall == null || methodCall.Arguments.Count <= 1 || !(methodCall.Object is ConstantExpression))
                throw new InvalidOperationException("Invalid action expression.");

            ConstantExpression constantExpression = (methodCall.Object as ConstantExpression);

            if (constantExpression == null)
                throw new InvalidOperationException("Invalid action expression.");

            MethodInfo methodInfo = constantExpression.Value as MethodInfo;

            if (methodInfo == null)
                throw new InvalidOperationException("Invalid action expression.");

            return methodInfo.Name;
        }

        public QueryBase CreateQueryFromUri(Uri uri, NameValueCollection requestForm)
        {
            try
            {
                return Query.CloneNoBasePath().From(uri, requestForm);
            }
            catch (TargetInvocationException)
            {
                throw new InvalidOperationException("Invalid Query");
            }
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (Query.IsMatch(httpContext.Request.Url))
            {
                RouteData routeData = new RouteData(this, RouteHandler);

                routeData.Values.Add(RouteValueKind.Controller, _controller);
                routeData.Values.Add(RouteValueKind.Action, _action);
                routeData.Values.Add(RouteValueKind.Query, CreateQueryFromUri(httpContext.Request.Url, httpContext.Request.Form));

                //routeData.DataTokens.Add(RouteTokenKind.Query, CreateQueryFromUri(new Uri(url)));

                return routeData;                
            }
            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            string controller = values[RouteValueKind.Controller] as string;
            string action = values[RouteValueKind.Action] as string;

            return (controller == _controller && action == _action) ? new VirtualPathData(this, StripLeadingSlash(Query)) : null; 
        }

        public string StripLeadingSlash(string url)
        {
            return url.Coalesce(s => s[0] == '/' ? s.Substring(1) : s);
        }
    }
}
