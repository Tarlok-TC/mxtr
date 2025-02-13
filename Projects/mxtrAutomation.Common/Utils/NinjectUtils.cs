using Ninject.Activation;

namespace mxtrAutomation.Common.Utils
{
    public static class NinjectUtils
    {
        public static bool LookingFor(IRequest request, string name)
        {
            if (request != null && request.Constraint != null && request.Constraint.Target != null)
            {
                object target = request.Constraint.Target;

                var nameProperty = target.GetType().GetField("name");

                if (nameProperty != null && (string)nameProperty.GetValue(target) == name)
                    return true;
            }

            return false;
        }
    }
}
