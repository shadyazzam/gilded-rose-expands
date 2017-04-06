using GildedRose.Entities;
using GildedRose.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Logic
{
    /// <summary>
    /// Purchase Service.
    /// </summary>
    public class PurchaseService : IPurchaseService
    {
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly IItemService _itemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseService" /> class.
        /// </summary>
        /// <param name="purchaseRepository">The purchase repository.</param>
        /// <param name="itemService">The item service.</param>
        public PurchaseService(IRepository<Purchase> purchaseRepository, IItemService itemService)
        {
            _purchaseRepository = purchaseRepository;
            _itemService = itemService;
        }

        /// <summary>
        /// Gets all purchases within a specified time.
        /// </summary>
        /// <returns>A collection of all purchases within a specified time.</returns>
        public IEnumerable<Purchase> GetPurchases(string userId, DateTime fromDate)
        {
            return _purchaseRepository.Get(p => p.UserId == userId && p.Date > fromDate);
        }

        /// <summary>
        /// Gets a purchase by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The purchase matching the specified id, or null if it doesn't exist.</returns>
        public Purchase GetPurchaseById(string userId, Guid purchaseId)
        {
            return _purchaseRepository.Get(p => p.UserId == userId && p.Id == purchaseId).FirstOrDefault();
        }

        /// <summary>
        /// Adds a record of a purchase for a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="purchaseRequestItems">Details of the purchase.</param>
        /// <returns></returns>
        public Purchase AddPurchase(string userId, IList<PurchaseRequestItem> purchaseRequestItems)
        {
            var validRequestItems = ValidatePurchaseRequest(purchaseRequestItems);

            if (!validRequestItems.Any())
                throw new ArgumentException("No purchase request items received.");

            Guid purchaseId = Guid.NewGuid();

            var items = validRequestItems.Select(i => _itemService.GetItemById(i.ItemId));

            // Filter out null items
            items = items.Where(i => i != null);

            if (!items.Any())
                throw new ArgumentException("No purchase request items received.");

            Purchase purchase = new Purchase();
            purchase.Date = DateTime.UtcNow;
            purchase.IsReturn = false;
            purchase.PurchasedItems = validRequestItems.Select(i =>
            {
                return new PurchasedItem()
                {
                    Id = Guid.NewGuid(),
                    ItemId = i.ItemId,
                    Quantity = i.Quantity,
                    PurchaseId = purchaseId,
                    Item = items.First(it => it.Id == i.ItemId),
                    UnitPrice = items.First(it => it.Id == i.ItemId).Price
                };
            }).ToList();
            purchase.UserId = userId;
            purchase.Id = purchaseId;

            _purchaseRepository.Create(purchase);

            // Update the stock levels of the purchased items
            foreach (var item in items)
                _itemService.UpdateStockLevel(item.Id, item.Stock - validRequestItems.First(i => i.ItemId == item.Id).Quantity);

            _purchaseRepository.Save();

            return purchase;
        }

        /// <summary>
        /// Validates the items in the purchase request.
        /// </summary>
        /// <param name="purchaseRequestItems">The items in the purchase request.</param>
        /// <returns>The filtered list of items (invalid items removed).</returns>
        public IList<PurchaseRequestItem> ValidatePurchaseRequest(IList<PurchaseRequestItem> purchaseRequestItems)
        {
            if (purchaseRequestItems == null)
                throw new ArgumentNullException("No purchase request items received.");

            // Filter out null items and those with 0 quantity
            purchaseRequestItems = purchaseRequestItems.Where(i => i != null && i.Quantity > 0).ToList();

            if (!purchaseRequestItems.Any())
                throw new ArgumentException("No purchase request items received.");

            // Is there enough stock?
            var stockLevels = _itemService.GetItemStockLevels();

            var unrecognizedItems = purchaseRequestItems.Where(i => !stockLevels.ContainsKey(i.ItemId));
            if (unrecognizedItems.Any())
                throw new ArgumentException("Unrecognized purchase request item(s) received: " + string.Join(", ", unrecognizedItems.Select(u => u.ItemId)));
            
            var insufficientStockItems = purchaseRequestItems.Where(i => i.Quantity > stockLevels[i.ItemId]);
            if (insufficientStockItems.Any())
                throw new ArgumentOutOfRangeException("Insufficiently stocked purchase request item(s) received: " + string.Join(", ", unrecognizedItems.Select(u => u.ItemId)));

            // Return valid items
            return purchaseRequestItems;
        }
    }
}
