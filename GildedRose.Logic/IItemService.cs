using GildedRose.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Logic
{
    /// <summary>
    ///  Item Service.
    /// </summary>
    public interface IItemService
    {
        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Item> GetItems();

        /// <summary>
        /// Gets items by category.
        /// </summary>
        /// <param name="category">The category identifier.</param>
        /// <returns>The items that fall into the category.</returns>
        IEnumerable<Item> GetItemsByCategory(Guid category);

        /// <summary>
        /// Gets item by id.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <returns>The item.</returns>
        Item GetItemById(Guid id);

        /// <summary>
        /// Gets the stock levels for all the items.
        /// </summary>
        /// <returns>The stock levels for all the items.</returns>
        IDictionary<Guid, int> GetItemStockLevels();

        /// <summary>
        /// Updates a particular item's stock level.
        /// </summary>
        /// <param name="itemId">The identifier of the item to update.</param>
        /// <param name="newStockLevel">The new stock level.</param>
        void UpdateStockLevel(Guid itemId, int newStockLevel);

        /// <summary>
        /// Sums the total of all the prices for the list of items.
        /// </summary>
        /// <param name="items">The items in question.</param>
        /// <returns>The total price of all the items.</returns>
        decimal GetTotalPrice(IList<PurchaseRequestItem> items);
    }
}
