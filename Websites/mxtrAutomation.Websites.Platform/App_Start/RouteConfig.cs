using System.Web.Mvc;
using System.Web.Routing;
using mxtrAutomation.Web.Common.Extensions;
using mxtrAutomation.Websites.Platform.Controllers;
using mxtrAutomation.Websites.Platform.Queries;

namespace mxtrAutomation.Websites.Platform.App_Start
{
    public partial class RouteConfig
    {
        static partial void RegisterViewRoutes(RouteCollection routes);

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("content/static/{htmlfile}.htm");
            routes.IgnoreRoute("{htmlfile}.htm");
            routes.IgnoreRoute("Images/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapWebQueryRoute<DefaultWebQuery, LoginController>(c => c.Default);
            routes.MapWebQueryRoute<LoginSubmitWebQuery, LoginController>(c => c.LoginSubmit);
            routes.MapWebQueryRoute<LogoutWebQuery, LoginController>(c => c.Logout);

            routes.MapWebQueryRoute<AdminAddAccountSubmitWebQuery, AdminAddAccountController>(c => c.AddAccountSubmit);
            routes.MapWebQueryRoute<AdminAddAccountAttributesSubmitWebQuery, AdminAddAccountController>(c => c.AddAccountAttributesSubmit);
            routes.MapWebQueryRoute<AdminAddAccountUserSubmitWebQuery, AdminAddAccountController>(c => c.AddAccountUserSubmit);

            routes.MapWebQueryRoute<AdminEditAccountSubmitWebQuery, AdminEditAccountController>(c => c.EditAccountSubmit);
            routes.MapWebQueryRoute<AdminEditAccountAttributesSubmitWebQuery, AdminEditAccountController>(c => c.EditAccountAttributesSubmit);

            RegisterViewRoutes(routes);

            routes.MapRoute("Default", "{controller}/{action}/{id}", new
            {
                controller = "AdminEditAccountController",
                action = "GoogleAnalytics",
                id = UrlParameter.Optional
            });

        }
    }
}