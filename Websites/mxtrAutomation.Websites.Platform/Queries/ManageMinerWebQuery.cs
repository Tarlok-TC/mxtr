using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    public class ManageMinerWebQuery : WebQueryBase
    {
        public static readonly string Route = "/manage-miner";
    }
    public class AssignDomainWebQuery : WebQueryBase
    {
        public static readonly string Route = "/assign-domain";
    }
    public class InsertUpdateCustomerWebQuery : WebQueryBase
    {
        public static readonly string Route = "/insertupdate-customer";
    }
    public class InsertUpdateBuildingWebQuery : WebQueryBase
    {
        public static readonly string Route = "/insertupdate-building";
    }
    public class AddUpdateCustomerDataFromJsonWebQuery : WebQueryBase
    {
        public static readonly string Route = "/addupdate-customer-data";
        public AddUpdateCustomerDataFromJsonWebQuery()
        {
            _accountObjectId = Parameters.Add<string>("accountObjectId", false);
            _mxtrObjectId = Parameters.Add<string>("mxtrObjectId", false);
            _fileName = Parameters.Add<string>("fileName", false);
        }

        public string AccountObjectId
        {
            get { return _accountObjectId.Value; }
            set { _accountObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectId;

        public string MxtrObjectId
        {
            get { return _mxtrObjectId.Value; }
            set { _mxtrObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _mxtrObjectId;

        public string FileName
        {
            get { return _fileName.Value; }
            set { _fileName.Value = value; }
        }
        private readonly UrlParameterBase<string> _fileName;

    }
    public class AddUpdateBuildingDataFromJsonWebQuery : WebQueryBase
    {
        public static readonly string Route = "/addupdate-building-data";
        public AddUpdateBuildingDataFromJsonWebQuery()
        {
            _accountObjectId = Parameters.Add<string>("accountObjectId", false);
            _mxtrObjectId = Parameters.Add<string>("mxtrObjectId", false);
            _fileName = Parameters.Add<string>("fileName", false);
        }

        public string AccountObjectId
        {
            get { return _accountObjectId.Value; }
            set { _accountObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectId;

        public string MxtrObjectId
        {
            get { return _mxtrObjectId.Value; }
            set { _mxtrObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _mxtrObjectId;

        public string FileName
        {
            get { return _fileName.Value; }
            set { _fileName.Value = value; }
        }
        private readonly UrlParameterBase<string> _fileName;
    }
    public class DeleteDuplicateCustomerWebQuery : WebQueryBase
    {
        public static readonly string Route = "/deleteduplicate-customers";
    }
    public class DeleteDuplicateBuildingWebQuery : WebQueryBase
    {
        public static readonly string Route = "/deleteduplicate-buildings";
    }

    public class HandleOldBuildingDataWebQuery : WebQueryBase
    {
        public static readonly string Route = "/handle-oldBuildingData";
    }

    public class SetOpportunityPipeLineWebQuery : WebQueryBase
    {
        public static readonly string Route = "/opportunity-pipeLine";
    }

    public class AssignCoordinatesToAccountWebQuery : WebQueryBase
    {
        public static readonly string Route = "/assign-coordinates";
    }

    public class LeadAnalyticalWebQuery : WebQueryBase
    {
        public static readonly string Route = "/lead-analytical";
    }
    public class SubscribeLeadUpdatesWebQuery : WebQueryBase
    {
        public static readonly string Route = "/subscribeToLeadUpdates";
        public SubscribeLeadUpdatesWebQuery()
        {
            _subscribeUrl = Parameters.Add<string>("subscribeUrl", false);
        }

        public string SubscribeUrl
        {
            get { return _subscribeUrl.Value; }
            set { _subscribeUrl.Value = value; }
        }
        private readonly UrlParameterBase<string> _subscribeUrl;
    }

    public class SSCreateDateFixWebQuery : WebQueryBase
    {
        public static readonly string Route = "/sscreatedatefix";
    }
}