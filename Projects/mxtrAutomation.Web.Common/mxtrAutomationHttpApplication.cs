using System;
using System.Web;
using System.Web.Security;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Utils;
using mxtrAutomation.Web.Common.Authentication;

namespace mxtrAutomation.Web.Common
{
    public class mxtrAutomationHttpApplication : HttpApplication
    {
        public void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (Context.User != null && Context.User is mxtrAutomationPrincipal)
                return;

            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null)
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, ex, "Forms authentication failed in Decrypt().");
                return;
            }

            if (authTicket == null)
                return;

            string mxtrUserObjectID = null;
            string mxtrAccountObjectID = null;
            string userName = null;
            string fullName = null;
            string selectedWorkspaceIDs = null;
            string role = null;
            string sharpspringUserName = null;
            string sharpspringPassword = null;

            if (authTicket != null)
            {
                string[] userData = authTicket.UserData.Split('|');

                mxtrUserObjectID = userData[0];
                mxtrAccountObjectID = userData[1];
                userName = userData[2];
                fullName = userData[3];
                selectedWorkspaceIDs = userData[4];

                if (userData.Length < 8) // Used to logout existing user and login again so that user role can be added in cookies
                {
                    removeCookies();
                    return;
                }
                role = userData[5];
                sharpspringUserName = userData[6];
                sharpspringPassword = userData[7];
            }

            if (string.IsNullOrEmpty(mxtrUserObjectID))
            {
                removeCookies();
                return;
                //throw new InvalidOperationException("User authentication failed (username = '{0}') because authentication cookies did not contain an AdTrakUserID.".With(authTicket.Name));
            }

            ImxtrAutomationIdentity id = new mxtrAutomationFormsIdentity(authTicket, mxtrUserObjectID, mxtrAccountObjectID, userName, fullName, selectedWorkspaceIDs, role,sharpspringUserName,sharpspringPassword);

            Context.User = new mxtrAutomationPrincipal(id, new string[0]);
        }

        private void removeCookies()
        {
            string[] cookiesToRemove = new[] { FormsAuthentication.FormsCookieName, FormsAuthentication.FormsCookieName + "Ex" };

            foreach (var cookieName in cookiesToRemove)
            {
                if (Request.Cookies[cookieName] != null)
                {
                    HttpCookie curCookie = new HttpCookie(cookieName);
                    curCookie.Expires = DateTime.Now.AddDays(-2);
                    curCookie.Domain = FormsAuthentication.CookieDomain;
                    Response.Cookies.Add(curCookie);
                }
            }
        }
    }
}
