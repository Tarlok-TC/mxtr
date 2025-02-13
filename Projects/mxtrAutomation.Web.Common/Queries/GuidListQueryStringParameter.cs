using System;

namespace mxtrAutomation.Web.Common.Queries
{
    public class GuidListQueryStringParameter : ListQueryStringParameter<Guid>
    {
        public GuidListQueryStringParameter(string propertyName, bool isRequired)
            : base(propertyName, isRequired, g => g.ToString(), s => new Guid(s))
        {
        }
    }
}
