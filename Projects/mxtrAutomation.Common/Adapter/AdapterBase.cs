namespace mxtrAutomation.Common.Adapter
{
    public abstract partial class AdapterBase<I, O> : IAdapter<I, O>
        where I : class
        where O : class, new()
    {
        public O Map(I input)
        {
            return Map(input, null);
        }

        public O Map(I input, O output)
        {
            if (input == null)
                return output;
            output = output ?? new O();
            MapInternal(input, ref output);
            return output;
        }

        /// <summary>
        /// A method that is called by Map.  Map handles checking for nulls
        /// on the input, and also handles null checking for output as well.
        /// That does not mean that output nulls or not permitted, it will be null
        /// if the input is null, but otherwise will be created.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public virtual void MapInternal(I input, ref O output) { }
    }
}
