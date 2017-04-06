using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Logic.Payment
{
    /// <summary>
    /// Represents the results of processing a payment.
    /// </summary>
    public class PaymentProcessResult
    {
        public bool Success { get; set; }
        public string ConfirmationCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
