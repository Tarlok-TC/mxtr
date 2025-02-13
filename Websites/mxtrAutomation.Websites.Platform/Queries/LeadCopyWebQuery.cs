using mxtrAutomation.Web.Common.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Queries
{
    public class LeadCopyWebQuery : WebQueryBase
    {
        public static readonly string Route = "/copy-lead";
        public LeadCopyWebQuery()
        {
            _clientObjectId = Parameters.Add<string>("clientObjectId", false);
            _leadObjectId = Parameters.Add<string>("leadObjectId", false);
        }

        public string ClientObjectId
        {
            get { return _clientObjectId.Value; }
            set { _clientObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _clientObjectId;

        public string LeadObjectId
        {
            get { return _leadObjectId.Value; }
            set { _leadObjectId.Value = value; }
        }
        private readonly UrlParameterBase<string> _leadObjectId;

        public bool CopiedToParent
        {
            get { return _copiedToParent.Value; }
            set { _copiedToParent.Value = value; }
        }
        private readonly UrlParameterBase<bool> _copiedToParent;
    }
}