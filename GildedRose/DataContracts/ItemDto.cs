using GildedRose.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GildedRose.DataContracts
{
    public class ItemDto
    {
        [JsonProperty(PropertyName = "href")]
        public string Href
        {
            get
            {
                return "http://localhost/gr/api/items/" + Id.ToString();
            }
        }

        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }
        [JsonProperty(PropertyName = "stock")]
        public int Stock { get; set; }

        [JsonProperty(PropertyName = "category")]
        public CategoryDto Category { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            ItemDto item = (ItemDto)obj;

            return Name == item.Name
                && Description == item.Description
                && Price == item.Price
                && Stock == item.Stock
                && Id == item.Id
                && Href == item.Href
                && Category.Equals(item.Category); 
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Name.GetHashCode();
            return hash;
        }
    }
}