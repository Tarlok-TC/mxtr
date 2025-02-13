using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.UI
{
    public class CssResource : ClientResourceBase<CssKindBase, CssCategoryKind>
    {
        public CssResource(string value, CssCategoryKind category) : base(value, category) {}

        public CssResource(string value, CssCategoryKind category, IEnumerable<CssKindBase> dependencies) : base(value, category, dependencies) { }
    }
}