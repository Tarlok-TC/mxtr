using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.UI
{
    public partial class ViewKind : CommonViewKind
    {
        public static readonly ViewKind MainLayout = new ViewKind("MainLayout");
        public static readonly ViewKind PublicLayout = new ViewKind("PublicLayout");

        public ViewKind(string value) : base(value) {}
    }
}
