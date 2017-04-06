using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Entities
{
    /// <summary>
    /// PurchasedItem entity.
    /// </summary>
    public class PurchasedItem : IEntity
    {
        public Guid Id { get; set; }
        public Guid PurchaseId { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Item Item { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            PurchasedItem item = (PurchasedItem)obj;

            return Id == item.Id
                && ItemId == item.ItemId
                && Item.Equals(item.Item)
                && PurchaseId == item.PurchaseId
                && Quantity == item.Quantity;
        }

        public override int GetHashCode()
        {
            return GetHashCode();
        }
    }
}
