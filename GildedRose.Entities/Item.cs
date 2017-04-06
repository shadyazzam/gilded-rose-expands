using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Entities
{
    /// <summary>
    /// Item entity.
    /// </summary>
    public class Item : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
