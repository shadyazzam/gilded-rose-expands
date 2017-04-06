using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GildedRose.Entities;
using GildedRose.Repositories;

namespace GildedRose.Logic
{
    /// <summary>
    /// Item Service.
    /// </summary>
    public class ItemService : IItemService
    {
        private readonly IRepository<Item> _itemRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemService" /> class.
        /// </summary>
        /// <param name="itemRepository">The item repository.</param>
        public ItemService(IRepository<Item> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>A collection of all Item entities</returns>
        public IEnumerable<Item> GetItems()
        {
            return _itemRepository.Get();
        }

        /// <summary>
        /// Gets an item by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The item matching the specified id, or null if it doesn't exist.</returns>
        public Item GetItemById(Guid id)
        {
            return _itemRepository.Get(id);
        }

        /// <summary>
        /// Gets items by category.
        /// </summary>
        /// <param name="cagtegoryId">The category identifier.</param>
        /// <returns>The items matching the specified category.</returns>
        public IEnumerable<Item> GetItemsByCategory(Guid categoryId)
        {
            return _itemRepository.Get(i => i.CategoryId == categoryId);
        }

        /// <summary>
        /// Gets items' stock levels.
        /// </summary>
        /// <returns>The stock level of each item.</returns>
        public IDictionary<Guid, int> GetItemStockLevels()
        {
            return _itemRepository.Get().ToDictionary(k => k.Id, v => v.Stock);
        }

        /// <summary>
        /// Update the stock level of a particular item.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="newStockLevel">The item's new stock level.</param>
        public void UpdateStockLevel(Guid itemId, int newStockLevel)
        {
            _itemRepository.Get(itemId).Stock = newStockLevel;
            _itemRepository.Save();
        }

        /// <summary>
        /// Gets the total price of some items.
        /// </summary>
        /// <param name="items">The items to evaluate.</param>
        /// <returns>The total cost for the itemas.</returns>
        public decimal GetTotalPrice(IList<PurchaseRequestItem> items)
        {
            return items.Sum(i => GetItemById(i.ItemId).Price * i.Quantity);
        }
    }
}
