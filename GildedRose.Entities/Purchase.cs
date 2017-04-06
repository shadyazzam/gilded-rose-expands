using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Entities
{
    /// <summary>
    /// Purchase entity.
    /// </summary>
    public class Purchase : IEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public bool IsReturn { get; set; }

        public virtual IList<PurchasedItem> PurchasedItems { get; set; }
    }
}
