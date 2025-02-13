using mxtrAutomation.Common.Ioc;

namespace mxtrAutomation.Common.Codebase
{
    public abstract class EnvironmentBase : IEnvironment
    {
        private static IEnvironment _environment;

        public static IEnvironment Current
        {
            get { return _environment ?? (_environment = ServiceLocator.Current.GetInstance<IEnvironment>()); }
            set { _environment = value; } // For unit testing...
        }

        public EnvironmentKind Environment { get; set; }
    }
}
