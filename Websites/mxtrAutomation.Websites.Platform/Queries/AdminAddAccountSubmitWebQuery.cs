using System;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Web.Common.Attributes;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class AdminAddAccountSubmitWebQuery : WebQueryBase<AccountProfileViewData>
    {
        public static readonly string Route = "/admin/add-account-submit";
    }
}