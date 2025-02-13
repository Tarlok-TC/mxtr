using System;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Util.Store;
using System.Web;
using mxtrAutomation.Common.Utils;
using Google.Apis.Analytics.v3;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class AppAuthFlowMetadata : FlowMetadata
    {
        public static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ConfigManager.AppSettings["ClientId"],
                    ClientSecret = ConfigManager.AppSettings["ClientSecret"],
                },
                Scopes = new[] {
                    AnalyticsService.Scope.AnalyticsReadonly, //View Google Analytics Data
                    AnalyticsService.Scope.Analytics, //view and manage your Google Analytics data

                },
                // TODO: maybe in a future post I'll demonstrate a new EFDataStore usage.
                DataStore = new FileDataStore(HttpContext.Current.Server.MapPath(ConfigManager.AppSettings["GAFilePath"]))
            });

        public override string GetUserId(Controller controller)
        {
            string objectId = Convert.ToString(controller.TempData["objectId"]);
            if (!string.IsNullOrEmpty(objectId))
                return objectId;
            return controller.User.Identity.Name;
        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}