
namespace mxtrAutomation.Common.Utils
{
    public static class mxtrAutomationMath
    {
        public static decimal CostPerLead(int leadCount, decimal budget)
        {
            if (leadCount < 1)
            {
                return budget;
            }

            return budget / leadCount;
        }

        public static double EntranceBounceRate(double bounces, double entrances)
        {
            return bounces / entrances;
        }

        public static double VisitBounceRate(double bounces, double visits)
        {
            return bounces / visits;
        }

        public static double EmailDeliveryRate(double delivered, double sent)
        {
            if (sent == 0)
                return 0;

            return delivered / sent;
        }

        public static double EmailOpenRate(double opened, double delivered)
        {
            if (delivered == 0)
                return 0;

            return opened / delivered;
        }

        public static double EmailClickRate(double clicked, double opened)
        {
            if (opened == 0)
                return 0;

            return clicked / opened;
        }

        public static double GetRate(int x, int y)
        {
            if (y == 0)
                return 0;

            return (double)x / (double)y;
        }

    }
}
