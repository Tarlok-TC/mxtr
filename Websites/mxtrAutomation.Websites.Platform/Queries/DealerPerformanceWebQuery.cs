using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class DealerPerformanceWebQuery : WebQueryBase
    {
        public static readonly string Route = "/shaw/dealer-performance";

        public DealerPerformanceWebQuery()
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

    public class DealerPerformanceDetailWebQuery : WebQueryBase
    {
        public static readonly string Route = "/shaw/dealerperformance-detail";
        public DealerPerformanceDetailWebQuery()
        {
            _startDate = Parameters.Add<DateTime>("startdate", false);
            _endDate = Parameters.Add<DateTime>("enddate", false);
            //_accountObjectIDs = Parameters.Add<string>("accountobjectids", false);
            _objectID = Parameters.Add<string>("id", true);
            _isAjax = Parameters.Add<bool>("isajax", false);
        }
        public string ObjectID
        {
            get { return _objectID.Value; }
            set { _objectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _objectID;

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

        //public string AccountObjectIDs
        //{
        //    get { return _accountObjectIDs.Value; }
        //    set { _accountObjectIDs.Value = value; }
        //}
        //private readonly UrlParameterBase<string> _accountObjectIDs;
    }
}