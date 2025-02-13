using System;
using System.Linq.Expressions;

namespace mxtrAutomation.Common.Adapter
{
    public interface IProjection<I, O>
    {
        Expression<Func<I, O>> Projection { get; }
    }
}
