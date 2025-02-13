using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Websites.Platform.Queries
{
    public class LeadDeleteWebQuery : WebQueryBase
    {
        public static readonly string Route = "/delete-lead";        

        public LeadDeleteWebQuery()
        {
            _clientObjectId = Parameters.Add<string>("clientObjectId", false);
            _leadId = Parameters.Add<long>("leadId", false);
        }

        public string ClientObjectId
        {
            get { return _clientObjectId.Value; }
            set { _clientObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _clientObjectId;

        public long LeadId
        {
            get { return _leadId.Value; }
            set { _leadId.Value = value; }
        }
        private readonly UrlParameterBase<long> _leadId;

    }
}