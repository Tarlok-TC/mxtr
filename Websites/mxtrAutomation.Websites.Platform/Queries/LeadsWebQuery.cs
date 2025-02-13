using System;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Web.Common.Attributes;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class LeadsWebQuery : WebQueryBase
    {
        public static readonly string Route = "/leads";

        public LeadsWebQuery()
        {
            _startDate = Parameters.Add<DateTime>("startdate", false);
            _endDate = Parameters.Add<DateTime>("enddate", false);
            _accountObjectID = Parameters.Add<string>("accountobjectid", false);
            _isAjax = Parameters.Add<bool>("isajax", false);
        }

        public string AccountObjectID
        {
            get { return _accountObjectID.Value; }
            set { _accountObjectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectID;
        public bool IsAjax
        {
            get { return _isAjax.Value; }
            set { _isAjax.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isAjax;

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
    }

    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class ShawLeadsWebQuery : WebQueryBase
    {
        public static readonly string Route = "/shaw/leads";

        public ShawLeadsWebQuery()
        {
            _startDate = Parameters.Add<DateTime>("startdate", false);
            _endDate = Parameters.Add<DateTime>("enddate", false);
            _accountObjectID = Parameters.Add<string>("accountobjectid", false);
            _isAjax = Parameters.Add<bool>("isajax", false);
        }

        public string AccountObjectID
        {
            get { return _accountObjectID.Value; }
            set { _accountObjectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectID;
        public bool IsAjax
        {
            get { return _isAjax.Value; }
            set { _isAjax.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isAjax;

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
    }


    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class ShawDealerLeadsWebQuery : WebQueryBase
    {
        public static readonly string Route = "/shaw/dealer-leads";

        public ShawDealerLeadsWebQuery()
        {
            _startDate = Parameters.Add<DateTime>("startdate", false);
            _endDate = Parameters.Add<DateTime>("enddate", false);
            _accountObjectID = Parameters.Add<string>("accountobjectid", false);
            _isAjax = Parameters.Add<bool>("isajax", false);
        }

        public string AccountObjectID
        {
            get { return _accountObjectID.Value; }
            set { _accountObjectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectID;
        public bool IsAjax
        {
            get { return _isAjax.Value; }
            set { _isAjax.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isAjax;

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
    }
}
