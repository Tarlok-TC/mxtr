using mxtrAutomation.Common.Adapter;

namespace mxtrAutomation.Common.Ioc
{
    public class CommonAdapterModule : AdapterModuleBase
    {
        public override void Load()
        {
            Bind(typeof(IProjection<,>)).ToMethod(ConstructIdentityProjection).When(ProjectionHasMatchingArguments);
        }
    }
}
