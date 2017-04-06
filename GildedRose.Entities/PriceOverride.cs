using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Entities
{
    /// <summary>
    /// PriceOverride entity.
    /// </summary>
    public class PriceOverride : IEntity
    {
        /// <summary>
        /// The ItemId.
        /// </summary>
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
