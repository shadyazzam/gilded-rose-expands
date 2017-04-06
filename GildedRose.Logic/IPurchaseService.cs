using GildedRose.Entities;
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
    public interface IPurchaseService
    {
        /// <summary>
        /// Gets all purchases for a given user that within a certain time.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="fromDate">The date after which to look for purchases.</param>
        /// <returns></returns>
        IEnumerable<Purchase> GetPurchases(string userId, DateTime fromDate);

        /// <summary>
        /// Gets details of a past purchase for a given user by purchase id.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="purchaseId">The identifier of the past purchase.</param>
        /// <returns></returns>
        Purchase GetPurchaseById(string userId, Guid purchaseId);

        /// <summary>
        /// Adds a record of a purchase for a given user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="purchaseRequestItems">Details of the purchase.</param>
        /// <returns></returns>
        Purchase AddPurchase(string userId, IList<PurchaseRequestItem> purchaseRequestItems);

        /// <summary>
        /// Validates the items in the purchase request.
        /// </summary>
        /// <param name="purchaseRequestItems">The items in the purchase request.</param>
        /// <returns>The filtered list of items (invalid items removed).</returns>
        IList<PurchaseRequestItem> ValidatePurchaseRequest(IList<PurchaseRequestItem> purchaseRequestItems);      
    }
}
