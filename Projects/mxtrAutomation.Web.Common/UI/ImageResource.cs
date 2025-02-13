namespace mxtrAutomation.Web.Common.UI
{
    public class ImageResource : ClientResourceBase<ImageKindBase, ImageCategoryKind>
    {
        public ImageResource(string value) : base(value, ImageCategoryKind.Images) { }
    }
}
