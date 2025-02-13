using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using mxtrAutomation.Websites.Platform.Models.Login.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;
using mxtrAutomation.Websites.Platform.UI;
using mxtrAutomation.Websites.Platform.ViewModelAdapters;
using Ninject;
using mxtrAutomation.Corporate.Data.Services;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Websites.Platform.Utils;
using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Websites.Platform.Helpers;
using System.Security.Principal;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class LoginController : PublicLayoutControllerBase
    {
        private readonly ILoginViewModelAdapter _viewModelAdapter;
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public LoginController(ILoginViewModelAdapter viewModelAdapter, IUserService userService, IAccountService accountService)
        {
            _viewModelAdapter = viewModelAdapter;
            _userService = userService;
            _accountService = accountService;
        }

        public ActionResult Default(DefaultWebQuery query)
        {
            return Redirect(new LoginWebQuery());
        }

        public ActionResult ViewPage(LoginWebQuery query)
        {
            if (User != null && User.IsAuthenticated)
            {
                return Redirect(RedirectionHelper(User.MxtrAccountObjectID));
            }
            else
            {
                LoginViewModel model = new LoginViewModel();
                string Url = Request.Url.Host.ToLower();
                if (Url.Contains("."))
                {
                    Url = Url.Replace("http://", "");
                    Url = Url.Replace("https://", "");
                    Url = Url.Replace("www", "");
                    mxtrAccount account = AccountService.GetBrandingLogoURL(Url.Substring(0, Url.IndexOf(".")));
                    if (account != null)
                        model = _viewModelAdapter.BuildLoginViewModel(account);
                    else
                        model = _viewModelAdapter.BuildLoginViewModel(new mxtrAccount());
                }

                // Handle...
                return View(ViewKind.Login, model, query);
            }
        }

        public ActionResult LoginSubmit(LoginSubmitWebQuery query)
        {
            bool success = false;
            string message = string.Empty;
            string redirect = new IndexWebQuery();
            KlipfolioDataModel KlipfolioDataModel = new KlipfolioDataModel();
            try
            {
                mxtrUserCookie user = _userService.Login(query.Username, query.Password);
                if (user != null)
                {
                    if (!_accountService.IsAccountActive(user.MxtrAccountID))
                    {
                        success = false;
                        message = "Account is not active";
                    }
                    else
                    {
                        redirect = RedirectionHelper(user.AccountObjectID);
                        success = true;

                        //Klipfolio SSO Implementation
                        mxtrUser mxtrUser = _userService.GetUserByUserObjectID(user.ObjectID);
                        KlipfolioUtils Klipfolio = new KlipfolioUtils();
                        mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(user.AccountObjectID);
                        if (!string.IsNullOrEmpty(mxtrAccount.KlipfolioCompanyID) && !string.IsNullOrEmpty(mxtrAccount.KlipfolioSSOSecretKey))
                        {
                            if (string.IsNullOrEmpty(mxtrUser.KlipfolioSSOToken))
                            {
                                string userDetails = @"{""expires"":""1539129600"",""email"":" + mxtrUser.Email + "}";
                                mxtrUser ObjmxtrUser = new mxtrUser()
                                {
                                    KlipfolioSSOToken = Klipfolio.GenerateSSOToken(mxtrAccount.KlipfolioSSOSecretKey, mxtrAccount.KlipfolioCompanyID, userDetails),
                                    ObjectID = user.ObjectID
                                };
                                _userService.UpdateUserKlipfolioSSOToken(ObjmxtrUser);
                                KlipfolioDataModel.KlipfolioCompanyID = mxtrAccount.KlipfolioCompanyID;
                                KlipfolioDataModel.KlipfolioSSOToken = ObjmxtrUser.KlipfolioSSOToken;
                            }
                            else
                            {
                                KlipfolioDataModel.KlipfolioCompanyID = mxtrAccount.KlipfolioCompanyID;
                                KlipfolioDataModel.KlipfolioSSOToken = mxtrUser.KlipfolioSSOToken;

                            }
                        }
                        SetAuthCookie(user.UserName, user.FullName, user.ObjectID, user.AccountObjectID, user.Role, false, user.SharpspringUserName, user.SharpspringPassword, Helper.GetDomainName());
                    }
                }
                else
                {
                    success = false;
                    message = "Incorrect username or password";
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                redirect = new LoginWebQuery();
            }
            return Json(new { Success = success, Message = message, Redirect = redirect, KlipfolioAuthData = KlipfolioDataModel });
        }

        private string RedirectionHelper(string accountObjectID)
        {
            string redirect = new IndexWebQuery();
            if (!String.IsNullOrEmpty(HttpContext.Request.QueryString["redirecturl"]))
            {
                return HttpContext.Request.QueryString["redirecturl"];
            }
            else if (Request.UrlReferrer != null && !String.IsNullOrEmpty(Request.UrlReferrer.Query))
            {
                redirect = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["redirecturl"];
                if (redirect.Contains(","))
                {
                    string[] arrRedirectUrl = redirect.Split(',');
                    redirect = arrRedirectUrl[0];
                }
                return redirect;
            }
            else
            {
                string homePageUrl = String.Empty;
                mxtrAccount userParentAccount = new mxtrAccount();
                mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(accountObjectID);
                if (mxtrAccount.AccountType == AccountKind.Organization.ToString() || mxtrAccount.AccountType.ToLower() == "reseller")
                {
                    homePageUrl = mxtrAccount.HomePageUrl;
                }
                else
                {
                    userParentAccount = AccountService.GetAccountByAccountObjectID(mxtrAccount.ParentAccountObjectID);
                    userParentAccount = GetParentAccount(userParentAccount);
                    homePageUrl = userParentAccount.HomePageUrl;
                }

                if (!String.IsNullOrEmpty(homePageUrl))
                {
                    redirect = homePageUrl;
                }

                return redirect;
            }
        }

        private mxtrAccount GetParentAccount(mxtrAccount parentAccount)
        {
            if (parentAccount.AccountType == AccountKind.Client.ToString() || parentAccount.AccountType == AccountKind.Group.ToString())
            {
                parentAccount = AccountService.GetAccountByAccountObjectID(parentAccount.ParentAccountObjectID);
                //call the function recursively until we get parent
                return GetParentAccount(parentAccount);
            }

            return parentAccount;
        }

        public ActionResult Logout(LogoutWebQuery query)
        {
            string redirectUrl = new LoginWebQuery();
            mxtrAccount mxtrAccount = _accountService.GetAccountByAccountObjectID(User.MxtrAccountObjectID);
            mxtrUser mxtrUser = _userService.GetUserByUserObjectID(User.MxtrUserObjectID);
            KlipfolioDataModel KlipfolioDataModel = new KlipfolioDataModel();
            KlipfolioDataModel.KlipfolioCompanyID = mxtrAccount.KlipfolioCompanyID;
            KlipfolioDataModel.KlipfolioSSOToken = mxtrUser.KlipfolioSSOToken;
            RemoveAuthCookie();
            Session["workspacesAccountIdsCache"] = null;
            Session["dateFilter"] = null;
            return Json(new { Redirect = redirectUrl, KlipfolioAuthData = KlipfolioDataModel });
        }

        protected void RemoveAuthCookie()
        {
            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

            string[] cookiesToRemove = new[] { FormsAuthentication.FormsCookieName, FormsAuthentication.FormsCookieName };
            string domainName = Helper.GetDomainName();
            foreach (var cookieName in cookiesToRemove)
            {
                if (Request.Cookies[cookieName] != null)
                {
                    HttpCookie curCookie = new HttpCookie(cookieName);
                    curCookie.Expires = DateTime.Now.AddDays(-2);
                    //curCookie.Domain = FormsAuthentication.CookieDomain;
                    curCookie.Domain = domainName;
                    Response.Cookies.Add(curCookie);
                }
            }
        }
    }
}
