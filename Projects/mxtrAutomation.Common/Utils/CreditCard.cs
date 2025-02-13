using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace mxtrAutomation.Common.Utils
{
    public class CreditCard
    {
        public string CardHolderName { get; private set; }
        public string Number { get; private set; }
        public int ExpirationMonth { get; private set; }
        public int ExpirationYear { get; private set; }
        public int Cvv2 { get; private set; }

        public string Street1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }

        public CreditCardType Type { get; private set; }

        private static readonly Regex notANumber = new Regex(@"[^\d]");

        // http://stackoverflow.com/questions/72768/how-do-you-detect-credit-card-type-based-on-number
        private static readonly Dictionary<CreditCardType, Regex> validations = new Dictionary<CreditCardType, Regex>()
        {
            { CreditCardType.Visa, new Regex(@"^4[0-9]{12}(?:[0-9]{3})?$") },
            { CreditCardType.MasterCard, new Regex(@"^5[1-5][0-9]{14}$") },
            { CreditCardType.AmericanExpress, new Regex(@"^3[47][0-9]{13}$") },
            { CreditCardType.Discover, new Regex(@"^6(?:011|5[0-9]{2})[0-9]{12}$") },
            { CreditCardType.DinersClub, new Regex(@"^3(?:0[0-5]|[68][0-9])[0-9]{11}$") },
            { CreditCardType.JCB, new Regex(@"^(?:2131|1800|35\d{3})\d{11}$") }
        };

        public CreditCard(string cardHolder, string number, int expirationMonth, int expirationYear, int cvv2, string street1, string city, string state, string zip, string phone, string country)
        {
            CardHolderName = cardHolder;
            Number = Sanitize(number);
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
            Cvv2 = cvv2;
            Type = GetType(number);
            Street1 = street1;
            City = city;
            State = state;
            Zip = zip;
            Phone = phone;
            Country = country;
        }

        public static string Sanitize(string cardNumber)
        {
            return notANumber.Replace(cardNumber, String.Empty);
        }

        public static CreditCardType GetType(string cardNumber)
        {
            string cleanCardNumber = Sanitize(cardNumber);

            foreach (var item in validations)
            {
                if (item.Value.IsMatch(cleanCardNumber))
                {
                    return item.Key;
                }
            }

            throw new ArgumentException("Credit card number is invalid.", "cardNumber");
        }

    }
}
