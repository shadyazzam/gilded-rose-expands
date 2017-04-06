using GildedRose.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GildedRose.DataContracts
{
    public class PurchasedItemDto
    {
        [JsonProperty(PropertyName = "href")]
        public string Href
        {
            get
            {
                return "http://localhost/gr/api/purchases/" + PurchaseId.ToString();
            }
        }


        [JsonIgnore]
        public Guid PurchaseId { get; set; }
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }
        [JsonProperty(PropertyName = "unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty(PropertyName = "item")]
        public ItemDto Item { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            PurchasedItemDto item = (PurchasedItemDto)obj;

            return Href == item.Href
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