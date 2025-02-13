using System;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Web.Common.Attributes;

namespace mxtrAutomation.Websites.Platform.Queries
{
    [RequireLogin(IsLoginRequired = true, RedirectBack = true)]
    public class LeadWebQuery : WebQueryBase
    {
        public static readonly string Route = "/lead";

        public LeadWebQuery()
        {
            _objectID = Parameters.Add<string>("id", true);
        }

        public string ObjectID
        {
            get { return _objectID.Value; }
            set { _objectID.Value = value; }
        }
        private readonly UrlParameterBase<string> _objectID;    
    }
}
