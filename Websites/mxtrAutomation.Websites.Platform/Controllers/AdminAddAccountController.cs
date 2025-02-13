using System;
using System.Web.Mvc;
using mxtrAutomation.Websites.Platform.Models.Admin.ViewModels;
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
using GoogleMaps.LocationServices;

namespace mxtrAutomation.Websites.Platform.Controllers
{
    public class AdminAddAccountController : MainLayoutControllerBase
    {
        private readonly IAdminAddAccountViewModelAdapter _viewModelAdapter;

        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        private readonly IAccountUtils _accountUtils;
        private readonly IUserUtils _userUtils;

        public AdminAddAccountController(IAdminAddAccountViewModelAdapter viewModelAdapter, IAccountService accountService,
            IAccountUtils accountUtils, IUserService userService, IUserUtils userUtils)
        {
            _viewModelAdapter = viewModelAdapter;
            _accountService = accountService;
            _userService = userService;
            _accountUtils = accountUtils;
            _userUtils = userUtils;
        }

        public ActionResult ViewPage(AdminAddAccountWebQuery query)
        {
            // Get data...
            mxtrAccount parentMxtrAccount = _accountService.GetAccountByAccountObjectID(query.ParentAccountObjectID);

            List<EZShredAccountDataModel> clients = new List<EZShredAccountDataModel>();
            if (parentMxtrAccount.AccountType != AccountKind.Group.ToString()) // No need to show market places for group
            {
                mxtrAccount parentAccount = new mxtrAccount();
                parentAccount = GetParentAccountOfLead(parentMxtrAccount);
                clients = _accountService.GetFlattenedChildClientAccounts(parentAccount.ObjectID);
            }

            // Adapt data...
            AdminAddAccountViewModel model =
                _viewModelAdapter.BuildAdminAddAccountViewModel(query.ParentAccountObjectID, parentMxtrAccount, clients);

            // Handle...
            return View(ViewKind.AdminAddAccount, model, query);
        }

        private mxtrAccount GetParentAccountOfLead(mxtrAccount parentAccount)
        {
            if (parentAccount.AccountType != AccountKind.Organization.ToString() && parentAccount.AccountType.ToLower() != "reseller")
            {
                parentAccount = _accountService.GetAccountByAccountObjectID(parentAccount.ParentAccountObjectID);
                //call the function recursively until we get parent
                return GetParentAccountOfLead(parentAccount);
            }

            return parentAccount;
        }
        public ActionResult AddAccountSubmit(AdminAddAccountSubmitWebQuery query)
        {
            mxtrAccount account = new mxtrAccount();
            CreateNotificationReturn notification = new CreateNotificationReturn();

            if (query.IsValid)
            {
                AccountProfileViewData accountData = query.GetValues();

                account = _accountUtils.ConvertToMxtrAccountDataModel(accountData);
                account.CreateDate = System.DateTime.Now.ToString();
                account.IsActive = true;

                var address = account.StreetAddress + ", " + account.Country + " " + account.ZipCode;
                var locationService = new GoogleLocationService();
                var point = locationService.GetLatLongFromAddress(address);
                if (point != null)
                {
                    account.Latitude = point.Latitude;
                    account.Longitude = point.Longitude;
                }
                notification = _accountService.CreateAccount(account);

                //notification.Success = true;
            }

            return Json(new { Success = notification.Success, MxtrAccountID = account.MxtrAccountID, AccountObjectID = notification.ObjectID });
        }

        public ActionResult AddAccountAttributesSubmit(AdminAddAccountAttributesSubmitWebQuery query)
        {
            mxtrAccount account = new mxtrAccount();
            CreateNotificationReturn notification = new CreateNotificationReturn();

            if (query.IsValid)
            {
                AccountAttributesViewData accountData = query.GetValues();

                account.ObjectID = accountData.ObjectID;
                account.MxtrAccountID = accountData.MxtrAccountID.ToString();
                account.SharpspringSecretKey = accountData.SharpspringSecretKey;
                account.SharpspringAccountID = accountData.SharpspringAccountID;
                account.BullseyeClientId = accountData.BullseyeClientId;
                account.BullseyeAdminApiKey = accountData.BullseyeAdminApiKey;
                account.BullseyeSearchApiKey = accountData.BullseyeSearchApiKey;
                account.WebsiteUrl = accountData.WebsiteUrl;
                account.EZShredIP = accountData.EZShredIP;
                account.EZShredPort = accountData.EZShredPort;
                account.KlipfolioCompanyID = accountData.KlipfolioCompanyID;
                account.KlipfolioSSOSecretKey = accountData.KlipfolioSSOSecretKey;
                account.DealerId = accountData.DealerId;
                account.Lead = accountData.SSLead;
                account.ContactMade = accountData.SSContact;
                account.ProposalSent = accountData.SSQuoteSent;
                account.Closed = accountData.SSClosed;
                account.WonNotScheduled = accountData.SSWonNotScheduled;

                notification = _accountService.CreateAccountAttributes(account);
            }

            return Json(new { Success = notification.Success, MxtrAccountID = account.MxtrAccountID, AccountObjectID = notification.ObjectID, EZShredIP = account.EZShredIP, EZShredPort = account.EZShredPort });
        }

        public ActionResult AddAccountUserSubmit(AdminAddAccountUserSubmitWebQuery query)
        {
            mxtrUser user = new mxtrUser();
            CreateNotificationReturn notification = new CreateNotificationReturn();

            if (query.IsValid)
            {
                //AccountUserViewData userData = query.GetValues();
                try
                {
                    AccountUserViewData userData = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountUserViewData>(query.UserAccountData);
                    userData.EZShredAccountMappings = userData.EZShredAccountMappings != null ? userData.EZShredAccountMappings.Where(x => !String.IsNullOrEmpty(x.EZShredId)).ToList() : new List<EZShredAccountDataModel>();

                    userData.AccountObjectID = query.AccountObjectID;
                    userData.MxtrAccountID = new Guid(query.MxtrAccountID);

                    user = _userUtils.ConvertToMxtrUserDataModel(userData);

                    user.CreateDate = System.DateTime.Now.ToString();
                    user.IsActive = true;
                    user.IsApproved = true;
                    user.IsLockedOut = false;
                    user.FailedLoginAttempts = 0;

                    if (String.IsNullOrEmpty(userData.ObjectID))
                    {
                        if (!_userService.IsUserExist(user.UserName))
                        {
                            notification = _userService.CreateUser(user);
                        }
                        else
                        {
                            return Json(new { Success = false, Message = "User name already exist" });
                        }
                    }
                    else
                    {
                        notification = _userService.UpdateUser(user);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            return Json(new { Success = notification.Success, MxtrUserID = user.MxtrUserID, UserObjectID = notification.ObjectID, Message = "" });
        }

        public ActionResult DeleteUser(AdminDeleteAccountUserWebQuery query)
        {
            CreateNotificationReturn notification = new CreateNotificationReturn();
            notification = _userService.DeleteUser(query.ObjectId);

            return Json(new { Success = notification.Success });
        }
    }
}
