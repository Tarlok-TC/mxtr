using mxtrAutomation.Api.Bullseye;
using mxtrAutomation.Api.EZShred;
using mxtrAutomation.Api.Services;
using mxtrAutomation.Api.Sharpspring;
using mxtrAutomation.Common.Ioc;
using System.Configuration;

namespace mxtrAutomation.Api.Ioc
{
    public class ApiAdapterModule : ServiceModuleBase
    {
        public override void Load()
        {
            //LoadSharpspringEndpoints();
            //Bind<ISharpspringService>().To<SharpspringService>().InSingletonScope();

            LoadBullseyeEndpoints();
            Bind<IBullseyeService>().To<BullseyeService>().InSingletonScope();

            //LoadEZShredEndpoints();
            //Bind<IEZShredService>().To<EZShredService>().InSingletonScope();
        }

        private void LoadSharpspringEndpoints()
        {
            Bind<ISharpspringApi>().To<SharpspringApi>()
                .WithConstructorArgument("argBaseUrl", ConfigurationManager.AppSettings["SharpspringBaseUrl"]);
        }

        private void LoadBullseyeEndpoints()
        {
            Bind<IBullseyeApi>().To<BullseyeApi>()
                .WithConstructorArgument("argBaseUrl", ConfigurationManager.AppSettings["BullseyeBaseUrl"]);
        }
        private void LoadEZShredEndpoints()
        {
            Bind<IEZShredApi>().To<EZShredApi>()
                .WithConstructorArgument("argBaseUrl", ConfigurationManager.AppSettings["EZShredBaseUrl"]);
        }

    }
}
