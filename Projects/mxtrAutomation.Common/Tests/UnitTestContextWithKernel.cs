using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Ioc;
using Ninject;

namespace mxtrAutomation.Common.Tests
{
    public abstract class UnitTestContextWithKernel : UnitTestContext
    {
        public IServiceLocator NinjectLocator { get; set; }

        public virtual IEnumerable<string> Assemblies
        {
            get
            {
                return new[] { "AdTrak360.Common", "AdTrak360.Web.Common", "AdTrak360.Data" };
            }
        }

        public override void SetupContext()
        {
            LoadKernel();
        }

        protected void LoadKernel()
        {
            IKernel kernel = new StandardKernel();

            if (!Assemblies.Try(assembly => Assembly.Load(assembly)))
                throw new InvalidOperationException("Failed to load all assemblies in UnitTestContextWithKernel.");

            IEnumerable<Assembly> assemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetType().Namespace != "System.Reflection.Emit");

            kernel.Load(assemblies);

            NinjectLocator = new NinjectServiceLocator(kernel as StandardKernel);

            ServiceLocator.SetLocatorProvider(() => NinjectLocator);
        }
    }
}
