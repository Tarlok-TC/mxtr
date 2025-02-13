using mxtrAutomation.Web.Common.Queries;
namespace mxtrAutomation.Websites.Platform.Queries
{
    public class NextDayRouteWebQuery : WebQueryBase
    {
        public static readonly string Route = "/NextDayRoute";
    }
    public class GetNextDayRouteTicketWebQuery : WebQueryBase
    {
        public static readonly string Route = "/NextDayRoute-ticket";
        public GetNextDayRouteTicketWebQuery()
        {
            _accountObjectId = Parameters.Add<string>("accountObjectId", false);
            _ezshredUserID = Parameters.Add<string>("EzshredUserID", false);
            _locationName = Parameters.Add<string>("LocationName", false);
        }

        public string EzshredUserID
        {
            get { return _ezshredUserID.Value; }
            set { _ezshredUserID.Value = value; }
        }
        private readonly UrlParameterBase<string> _ezshredUserID;
        public string AccountObjectId
        {
            get { return _accountObjectId.Value; }
            set { _accountObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectId;
        public string LocationName
        {
            get { return _locationName.Value; }
            set { _locationName.Value = value; }
        }
        private readonly UrlParameterBase<string> _locationName;
    }
    public class CheckNextDayRouteTicketWebQuery : WebQueryBase
    {
        public static readonly string Route = "/check-NextDayRouteTicket";
        public CheckNextDayRouteTicketWebQuery()
        {
            _accountObjectId = Parameters.Add<string>("accountObjectId", false);
            _ezshredUserID = Parameters.Add<string>("EzshredUserID", false);
        }

        public string EzshredUserID
        {
            get { return _ezshredUserID.Value; }
            set { _ezshredUserID.Value = value; }
        }
        private readonly UrlParameterBase<string> _ezshredUserID;
        public string AccountObjectId
        {
            get { return _accountObjectId.Value; }
            set { _accountObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _accountObjectId;
    }
}