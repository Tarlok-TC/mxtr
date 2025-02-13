using System;
using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class RetailersOverviewWebQuery : WebQueryBase
    {
        public static readonly string Route = "/retailers";

        public RetailersOverviewWebQuery()
        {
            _startDate = Parameters.Add<DateTime>("startdate", false);
            _endDate = Parameters.Add<DateTime>("enddate", false);
            _accountObjectIDs = Parameters.Add<string>("accountobjectids", false);
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
    
    }
}
