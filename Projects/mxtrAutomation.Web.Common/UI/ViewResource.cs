using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.UI
{
    public class ViewResource : ClientResourceBase<ViewKindBase, ViewCategoryKind>
    {
        public ViewResource(string value, ViewCategoryKind category) : base(value, category) {}

        public ViewResource(string value, ViewCategoryKind category, IEnumerable<ViewKindBase> dependencies) : base(value, category, dependencies) {}
    }
}