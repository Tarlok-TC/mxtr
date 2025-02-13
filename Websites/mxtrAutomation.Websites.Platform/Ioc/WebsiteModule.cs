using System.Collections.Generic;
using mxtrAutomation.Common.Attributes;
using mxtrAutomation.Common.Codebase;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Services;
using mxtrAutomation.Common.Utils;
using mxtrAutomation.Web.Common.UI;
using mxtrAutomation.Websites.Platform.UI;
using Ninject.Modules;
using mxtrAutomation.Websites.Platform.Utils;

namespace mxtrAutomation.Websites.Platform.Ioc
{
    public class WebsiteModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IEnvironment>().To<WebEnvironment>().InSingletonScope();
            Bind<IClientResourceCollection<CssKindBase, CssCategoryKind>>().To<CssCollection>().InSingletonScope();
            Bind<IClientResourceCollection<JSKindBase, JSCategoryKind>>().To<JSCollection>().InSingletonScope();
            Bind<IClientResourceCollection<ImageKindBase, ImageCategoryKind>>().To<ImageCollection>().InSingletonScope();
            Bind<IViewCollection>().To<ViewCollection>().InSingletonScope();
            Bind<IAccountUtils>().To<AccountUtils>().InSingletonScope();
            Bind<IUserUtils>().To<UserUtils>().InSingletonScope();

            //Bind<IEmailer>().ToMethod(x => (EnvironmentBase.Current.Environment == EnvironmentKind.Production) ? new AdTrakEmailer() as IEmailer : new DevEmailer()).InSingletonScope();
            //Bind<IEmailer>().To<DevEmailer>().When(r => NinjectUtils.LookingFor(r, "dev")).Named("dev");

        }
    }
}