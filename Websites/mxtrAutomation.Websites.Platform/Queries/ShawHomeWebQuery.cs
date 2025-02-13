using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class ShawHomeWebQuery : WebQueryBase
    {
        public static readonly string Route = "/Shaw/Home";

        public ShawHomeWebQuery()
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
    public class GetDealerDataWebQuery : WebQueryBase
    {
        public static readonly string Route = "/shaw/get-dealer-data";
        public GetDealerDataWebQuery()
        {
            _startDate = Parameters.Add<DateTime>("startdate", false);
            _endDate = Parameters.Add<DateTime>("enddate", false);
            _accountObjectIDs = Parameters.Add<string>("accountobjectids", false);
            _isSearchCall = Parameters.Add<bool>("isSearchCall", false);
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

        public bool IsSearchCall
        {
            get { return _isSearchCall.Value; }
            set { _isSearchCall.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isSearchCall;
    }

    public class GetLeadsChartDataWebQuery : WebQueryBase
    {
        public static readonly string Route = "/shaw/get-leadchart-data";
        public GetLeadsChartDataWebQuery()
        {
            _startDate = Parameters.Add<DateTime>("startdate", false);
            _endDate = Parameters.Add<DateTime>("enddate", false);
            _accountObjectIDs = Parameters.Add<string>("accountobjectids", false);
            _isSearchCall = Parameters.Add<bool>("isSearchCall", false);
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

        public bool IsSearchCall
        {
            get { return _isSearchCall.Value; }
            set { _isSearchCall.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isSearchCall;
    }
}