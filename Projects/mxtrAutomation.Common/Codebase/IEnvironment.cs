namespace mxtrAutomation.Common.Codebase
{
    public enum EnvironmentKind
    {
        Development,
        Production
    }

    public interface IEnvironment
    {
        EnvironmentKind Environment { get; set; }
    }
}
