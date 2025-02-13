using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.UI
{
    public class JSResource : ClientResourceBase<JSKindBase, JSCategoryKind>
    {
        public JSResource(string value) : base(value, JSCategoryKind.Default) { }

        public JSResource(string value, IEnumerable<JSKindBase> dependencies) : base(value, JSCategoryKind.Default, dependencies) { }
    }
}