using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using mxtrAutomation.Web.Common.Authentication;
using Ninject;
using mxtrAutomation.Web.Common.Helpers;

namespace mxtrAutomation.Web.Common.UI
{
    public abstract class mxtrAutomationControllerBase : Controller
    {
        [Inject]
        public IViewCollection ViewCollection { get; set; }

        public new ImxtrAutomationIdentity User
        {
            get { return base.User.Identity as ImxtrAutomationIdentity; }
        }

        public virtual ActionResult View(ViewKindBase viewKind, ViewModelBase model)
        {
            return View(ViewCollection[viewKind].Value, model);
        }

        public virtual ActionResult Redirect(ViewKindBase viewKind)
        {
            return Redirect(ViewCollection[viewKind].Value);
        }

        protected string RenderView(ViewKindBase viewKind, ViewKindBase masterKind, object model)
        {
            string viewName = ViewCollection[viewKind].Value;
            string masterName = ViewCollection[masterKind].Value;

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult =
                    ViewEngines.Engines.FindView(ControllerContext, viewName, masterName);

                ViewContext viewContext =
                    new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        protected string RenderPartial(ViewKindBase viewKind, object model)
        {
            string viewName = ViewCollection[viewKind].Value;

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult =
                    ViewEngines.Engines.FindPartialView(ControllerContext, viewName);

                ViewContext viewContext =
                    new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public new ActionResult Json(object data)
        {
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected void SetAuthCookie(string userName, string fullName, string mxtrUserObjectID, string mxtrAccountObjectID, string role, bool isKeepLoggedIn, string sharpSpringUserName, string sharpSpringPassword, string domainName)
        {
            //selected workspace ids ({4} defaults to just the logged in account upon login
            string exData = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", mxtrUserObjectID, mxtrAccountObjectID, userName, fullName, mxtrAccountObjectID, role, sharpSpringUserName, sharpSpringPassword);

            // Create our tickets with our user information
            var authenticationTickets =
                new[]
                    {
                        new
                            {
                                CookieName = FormsAuthentication.FormsCookieName,
                                Ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), true, exData)
                            }
                    };

            DateTime expires = isKeepLoggedIn ? DateTime.Now.AddDays(7) : DateTime.Now.Add(FormsAuthentication.Timeout);

            foreach (var ticket in authenticationTickets)
            {
                HttpCookie authCookie =
                    new HttpCookie(ticket.CookieName, FormsAuthentication.Encrypt(ticket.Ticket))
                    {
                        Expires = expires,
                        //Domain = FormsAuthentication.CookieDomain
                        Domain = domainName,
                    };

                // Load the cookie into the response
                if (Response.Cookies[ticket.CookieName] != null)
                    Response.SetCookie(authCookie);
                else
                    Response.Cookies.Add(authCookie);
            }
        }

        protected FormsAuthenticationTicket GetAuthTicket()
        {
            FormsAuthenticationTicket retVal = null;
            HttpCookie curCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (curCookie != null && !String.IsNullOrWhiteSpace(curCookie.Value))
            {
                retVal = FormsAuthentication.Decrypt(curCookie.Value);
            }

            return retVal;
        }
    }
}