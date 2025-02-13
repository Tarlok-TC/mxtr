using System;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Websites.Platform.Models.Account.ViewData;
using mxtrAutomation.Web.Common.Attributes;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class AdminDeleteAccountUserWebQuery : WebQueryBase
    {
        public static readonly string Route = "/admin/delete-user";

        public AdminDeleteAccountUserWebQuery()
        {
            _objectId = Parameters.Add<string>("objectid", false);
        }

        public string ObjectId
        {
            get { return _objectId.Value; }
            set { _objectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _objectId;
    }
}