using System;
using System.Linq.Expressions;

namespace mxtrAutomation.Common.Adapter
{
    public class ProjectionAdapter<I, O> : AdapterBase<I, O>, IProjectionAdapter<I, O>
        where I : class
        where O : class, new()
    {
        public virtual Expression<Func<I, O>> Projection { get; protected set; }

        // ReSharper disable RedundantAssignment
        public override void MapInternal(I input, ref O output)
        {
            output = Projection.Compile().Invoke(input);
        }
        // ReSharper restore RedundantAssignment
    }
}
