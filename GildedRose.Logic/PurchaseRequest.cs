using GildedRose.Logic.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Logic
{
    /// <summary>
    /// A purchase request from a user.
    /// </summary>
    public class PurchaseRequest
    {
        /// <summary>
        /// The items in the purchase request.
        /// </summary>
        public IList<PurchaseRequestItem> Items { get; set; }
        /// <summary>
        /// Payment information to complete the transaction.
        /// </summary>
        public PaymentInfo PaymentInfo { get; set; }
    }
}
