using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Web.Common.UI
{
    public abstract class ClientResourceBase<KIND, CATEGORY>
    {
        public string Value { get; set; }

        public CATEGORY Category { get; set; }
        public bool ShouldBundle { get; set; }
        public IEnumerable<KIND> Dependencies { get; set; }

        public bool IsLocalFile
        {
            get { return Value.Coalesce(x => x.StartsWith("/")); }
        }

        protected ClientResourceBase(string value, CATEGORY category)
        {
            Value = value;
            Category = category;
            ShouldBundle = true;
        }

        protected ClientResourceBase(string value, CATEGORY category, IEnumerable<KIND> dependencies)
        {
            Value = value;
            Category = category;
            ShouldBundle = true;
            Dependencies = dependencies;
        }
    }
}
