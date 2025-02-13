using System;
using mxtrAutomation.Web.Common.Queries;

namespace mxtrAutomation.Web.Common.Helpers
{
    public partial class ViewLink
    {
        public string Text { get; set; }
        public virtual string Url { get; set; }
        public string OnClick { get; set; }
        public virtual bool IsExternal { get; set; }
        public virtual string CssClass { get; set; }
        public virtual WebImage Image { get; set; }
    }

    public class ViewLink<Q> : ViewLink
        where Q : QueryBase
    {
        public Q Query
        {
            get
            {
                if (QueryIsInvalid)
                    throw new Exception("The query is in an invalid state, because the Url has been set directly on the object.");
                return _query;
            }
            set
            {
                _query = value;
                Url = _query != null ? _query.ToString() : string.Empty;
            }
        }
        private Q _query;

        public bool QueryIsInvalid { get; private set; }

        public override string Url
        {
            get { return base.Url; }
            set
            {
                base.Url = value;
                QueryIsInvalid = true;
            }
        }

        public ViewLink() { }

        public ViewLink(Q query)
        {
            Query = query;
            QueryIsInvalid = false;
        }
    }
}
