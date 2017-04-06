using GildedRose.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GildedRose.DataContracts
{
    public class PurchaseDto
    {
        [JsonProperty(PropertyName = "href")]
        public string Href
        {
            get
            {
                return "http://localhost/gr/api/purchases/" + Id.ToString();
            }
        }

        [JsonProperty(PropertyName = "confirmationNumber")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
        [JsonProperty(PropertyName = "isReturn")]
        public bool IsReturn { get; set; }

        [JsonProperty(PropertyName = "items")]
        public IEnumerable<PurchasedItemDto> PurchasedItems { get; set; }

        [JsonProperty(PropertyName = "totalPrice")]
        public decimal TotalPrice { get
            {
                return PurchasedItems.Sum(p => p.UnitPrice);
            }
        }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            PurchaseDto purchase = (PurchaseDto)obj;

            return Href == purchase.Href
                && Id == purchase.Id
                && IsReturn == purchase.IsReturn
                && TotalPrice == purchase.TotalPrice
                && PurchasedItems.SequenceEqual(purchase.PurchasedItems);
        }

        public override int GetHashCode()
        {
            return GetHashCode();
        }
    }
}