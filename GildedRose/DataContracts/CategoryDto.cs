using GildedRose.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GildedRose.DataContracts
{
    public class CategoryDto
    {
        [JsonProperty("href")]
        public string Href
        {
            get
            {
                return "http://localhost/gr/api/categories/" + CategoryId.ToString() + "/items";
            }
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonIgnore]
        public Guid CategoryId { get; set; }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            CategoryDto category = (CategoryDto)obj;

            return CategoryId == category.CategoryId
                && Name == category.Name
                && Href == category.Href;
        }

        public override int GetHashCode()
        {
            return GetHashCode();
        }
    }
}