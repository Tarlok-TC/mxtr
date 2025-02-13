using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Web.Common.Helpers
{
    public class AdTrakWebImage : WebImage
    {
        protected IClientResourceCollection<ImageKindBase, ImageCategoryKind> ImageCollection { get { return _imageCollection.Value; } }

        protected static LazyInjected<IClientResourceCollection<ImageKindBase, ImageCategoryKind>> _imageCollection =
            new LazyInjected<IClientResourceCollection<ImageKindBase, ImageCategoryKind>>();

        public ImageKindBase ImageKind { get; private set; }

        public override string BasePath
        {
            get { return "/Images"; }
        }

        public override string FileName
        {
            get { return ImageCollection.Coalesce(c => c[ImageKind].Value); }
        }

        public AdTrakWebImage(ImageKindBase imageKind)
        {
            ImageKind = imageKind;
        }
    }
}
