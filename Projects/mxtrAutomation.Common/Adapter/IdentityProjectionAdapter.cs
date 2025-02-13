using System;
using System.Linq.Expressions;

namespace mxtrAutomation.Common.Adapter
{
    public class IdentityProjectionAdapter<I> : ProjectionAdapter<I, I>
        where I : class, new()
    {
        public override Expression<Func<I, I>> Projection { get { return x => x; } }
    }
}
