using mxtrAutomation.Web.Common.Queries;
using System;

namespace mxtrAutomation.Websites.Platform.Queries
{
    public class ManageMinerDeleteSharpspringLogWebQuery : WebQueryBase
    {
        public static readonly string Route = "/delete-sharpspring-log";
        public ManageMinerDeleteSharpspringLogWebQuery()
        {
            _lastEndDateForDataCollection = Parameters.Add<DateTime>("lastEndDateForDataCollection", false);
            _minerType = Parameters.Add<string>("minerType", false);
            _isAjax = Parameters.Add<bool>("isajax", false);
            _accountObjectIDs = Parameters.Add<string>("accountObjectIDs", false);
        }

        public DateTime LastEndDateForDataCollection
        {
            get { return _lastEndDateForDataCollection.Value; }
            set { _lastEndDateForDataCollection.Value = value; }
        }
        private readonly UrlParameterBase<DateTime> _lastEndDateForDataCollection;


        public string MinerType
        {
            get { return _minerType.Value; }
            set { _minerType.Value = value; }
        }
        private readonly UrlParameterBase<string> _minerType;

        public bool IsAjax
        {
            get { return _isAjax.Value; }
            set { _isAjax.Value = value; }
        }
        private readonly UrlParameterBase<bool> _isAjax;

        public string AccountObjectIDs
        {
            get { return _accountObjectIDs.Value; }
            set { _accountObjectIDs.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectIDs;

    }
}