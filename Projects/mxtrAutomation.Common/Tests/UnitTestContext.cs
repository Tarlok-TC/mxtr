using NUnit.Framework;

namespace mxtrAutomation.Common.Tests
{
    public abstract class UnitTestContext
    {
        public abstract void SetupContext();
        public abstract void Act();
        public virtual void PostAct() { }
        public virtual void ExtendContext() { }

        [SetUp]
        public virtual void SetUp()
        {
            SetupContext();

            ExtendContext();

            try
            {
                Act();
            }
            finally
            {
                PostAct();
            }
        }
    }
}
