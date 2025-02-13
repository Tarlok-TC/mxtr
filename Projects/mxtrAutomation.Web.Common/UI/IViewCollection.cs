using System.Collections.Generic;

namespace mxtrAutomation.Web.Common.UI
{
    public interface IViewCollection : IDictionary<ViewKindBase, ClientResourceBase<ViewKindBase, ViewCategoryKind>> { }
}
