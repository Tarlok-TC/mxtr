using System;
using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class DashboardWebQuery : WebQueryBase
    {
        public static readonly string Route = "/dashboard";

        public DashboardWebQuery()
        {
            _startDate = Parameters.Add<DateTime>("startdate", false);
            _endDate = Parameters.Add<DateTime>("enddate", false);
            _accountObjectIDs = Parameters.Add<string>("accountobjectids", false);
            _isAjax = Parameters.Add<bool>("isajax", false);
        }

        public DateTime StartDate
        {
            get { return _startDate.Value; }
            set { _startDate.Value = value; }
        }
        private readonly UrlParameterBase<DateTime> _startDate;

        public DateTime EndDate
        {
            get { return _endDate.Value; }
            set { _endDate.Value = value; }
        }
        private readonly UrlParameterBase<DateTime> _endDate;

        public string AccountObjectIDs
        {
            get { return _accountObjectIDs.Value; }
            set { _accountObjectIDs.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectIDs;

        public bool IsAjax
        {
            get { return _isAjax.Value; }
            set { _isAjax.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isAjax;
    }
}
