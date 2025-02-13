
namespace mxtrAutomation.Common.Utils
{
    public class MaskedCreditCard
    {
        public string MaskedNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public CreditCardType Type { get; set; }
    }
}
