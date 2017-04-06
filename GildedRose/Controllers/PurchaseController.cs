using GildedRose.DataContracts;
using GildedRose.Entities;
using GildedRose.Logic;
using GildedRose.Logic.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace GildedRose.Controllers
{
    [Authorize]
    public class PurchaseController : ApiController
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IPaymentService _paymentService;
        private readonly IItemService _itemService;
        private readonly IUserService _userService;

        public string CurrentUserId
        {
            get
            {
                return _userService.GetCurrentUserId();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseController" /> class.
        /// </summary>
        /// <param name="purchaseService">The purchase business logic service.</param>
        public PurchaseController(IPurchaseService purchaseService, IPaymentService paymentService, IItemService itemService, IUserService userService)
        {
            _purchaseService = purchaseService;
            _paymentService = paymentService;
            _itemService = itemService;
            _userService = userService;
        }

        /// <summary>
        /// Gets the current user's purchases for the last year.
        /// </summary>
        /// <returns>The purchases.</returns>
        [HttpGet]
        [Route("api/purchases")]
        public IEnumerable<PurchaseDto> GetPurchasesForThePastYear()
        {
            return _purchaseService
                .GetPurchases(CurrentUserId, DateTime.UtcNow.AddYears(-1))
                .Select(p => p.ToDto());
        }

        /// <summary>
        /// Gets the specified purchase.
        /// </summary>
        /// <param name="purchaseId">The purchase identifier.</param>
        /// <returns>The specified purchase.</returns>
        [HttpGet]
        [Route("api/purchases/{purchaseId}", Name = "PurchaseByPurchaseId")]
        public IHttpActionResult GetPurchaseById(Guid purchaseId)
        {
            var purchase = _purchaseService.GetPurchaseById(CurrentUserId, purchaseId);
            if (purchase == null) return NotFound();

            return Ok(purchase.ToDto());
        }

        /// <summary>
        /// Makes a purchase.
        /// </summary>
        /// <param name="cagtegoryId">The category identifier.</param>
        /// <returns>Summary of the new purchase.</returns>
        [HttpPost]
        [Route("api/purchases")]
        public async Task<IHttpActionResult> MakePurchase([FromBody] PurchaseRequest request)
        {
            try
            {
                // Verify the model state is valid.
                if (!ModelState.IsValid)
                    return BadRequest("Invalid purchase request.");

                // Validate the purchase request information.
                _purchaseService.ValidatePurchaseRequest(request.Items);

                // Get the total price of the requested items.
                decimal amount = _itemService.GetTotalPrice(request.Items);
                
                // Process the payment.
                var paymentResult = await _paymentService.ProcessPayment(amount, request.PaymentInfo);

                if (!paymentResult.Success)
                    return BadRequest("Payment failure: " + paymentResult.ErrorMessage);
                
                // Add a record of the completed purchase.
                Purchase newPurchase = _purchaseService.AddPurchase(CurrentUserId, request.Items);
                return CreatedAtRoute("PurchaseByPurchaseId", new { purchaseId = newPurchase.Id }, newPurchase.ToDto());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
