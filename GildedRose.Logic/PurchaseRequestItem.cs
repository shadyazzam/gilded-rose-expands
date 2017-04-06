using GildedRose.Logic.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Logic
{
    /// <summary>
    /// An item in a purchase request.
    /// </summary>
    public class PurchaseRequestItem
    {
        /// <summary>
        /// The identifier of the item.
        /// </summary>
        public Guid ItemId { get; set; }
        /// <summary>
        /// The desired quantity to purchase.
        /// </summary>
        public int Quantity { get; set; }
    }
}
