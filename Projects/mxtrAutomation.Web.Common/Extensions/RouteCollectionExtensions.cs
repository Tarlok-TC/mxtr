using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class RouteCollectionExtensions
    {
        public static void MapWebQueryRoute<QUERY, CONTROLLER>(this RouteCollection routes, Expression<Func<CONTROLLER, Func<QUERY, ActionResult>>> actionExpression)
            where QUERY : QueryBase, new()
            where CONTROLLER : Controller
        {
            routes.Add(typeof(QUERY).Name, new WebQueryRoute<QUERY, CONTROLLER>(actionExpression));
        }

        public static void MapStaticViewWebQueryRoute<QUERY, CONTROLLER>(this RouteCollection routes, Expression<Func<CONTROLLER, Func<QUERY, ActionResult>>> actionExpression)
            where QUERY : QueryBase, new()
            where CONTROLLER : Controller
        {
            routes.Add(typeof(QUERY).Name, new WebQueryRoute<QUERY, CONTROLLER>(actionExpression));
        }

    }
}
