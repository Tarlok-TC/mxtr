using System;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Web.Common.Attributes;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class AdminAddAccountAttributesSubmitWebQuery : WebQueryBase<AccountAttributesViewData>
    {
        public static readonly string Route = "/admin/add-account-attributes-submit";
    }
}