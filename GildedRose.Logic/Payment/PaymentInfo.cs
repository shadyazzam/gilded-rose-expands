using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Logic.Payment
{
    /// <summary>
    /// Payment info required to process a payment.
    /// </summary>
    public class PaymentInfo
    {
        public CreditCard Card { get; set; }
        public string CardHolderName { get; set; }
        public long CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public int CardSecurityCode { get; set; }
        public string BillingAddress { get; set; }
    }
}
