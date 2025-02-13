namespace mxtrAutomation.Common.Adapter
{
    /// <summary />
    /// <typeparam name="I">The input type (the newer version)</typeparam>
    /// <typeparam name="O">The output type (the older version)</typeparam>
    public interface IAdapter<in I, out O>
    {
        O Map(I input);
    }
}
