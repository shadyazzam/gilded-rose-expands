using GildedRose.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GildedRose.DataContracts
{
    public static class DtoExtensions
    {
        public static ItemDto ToDto(this Item item)
        {
            return new ItemDto()
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Stock = item.Stock,
                Category = item.Category.ToDto()
            };
        }

        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto()
            {
                Name = category.Name,
                CategoryId = category.Id
            };
        }

        public static PurchaseDto ToDto(this Purchase purchase)
        {
            return new PurchaseDto()
            {
                Id = purchase.Id,
                Date = purchase.Date,
                IsReturn = purchase.IsReturn,
                PurchasedItems = purchase.PurchasedItems.Select(p => p.ToDto())
            };
        }

        public static PurchasedItemDto ToDto(this PurchasedItem item)
        {
            return new PurchasedItemDto()
            {
                Item = item.Item.ToDto(),
                PurchaseId = item.PurchaseId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
        }
    }
}