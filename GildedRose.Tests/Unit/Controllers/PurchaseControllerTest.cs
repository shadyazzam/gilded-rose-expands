using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GildedRose;
using GildedRose.Controllers;
using GildedRose.Logic;
using NSubstitute;
using GildedRose.Entities;
using GildedRose.DataContracts;
using System.Web.Http.Results;
using GildedRose.Logic.Payment;
using System.Threading.Tasks;

namespace GildedRose.Tests.Unit.Controllers
{
    [TestClass]
    public class PurchaseControllerTest : BaseTest
    {
        // Dependencies
        List<string> _userIds;
        List<Purchase> _purchases;
        PurchaseController _controller;
        IPurchaseService _purchaseService;
        IPaymentService _paymentService;
        
        [TestMethod]
        [TestCategory("ControllerTests")]
        public void GetPurchases_ShouldReturnAllUserPurchasesForLastYear()
        {
            var currentUserId = _userIds.First();
            var expectedPurchases = _purchases.Where(p => p.UserId == currentUserId && p.Date > DateTime.UtcNow.AddYears(-1))
                .Select(p => p.ToDto()).ToList();

            var result = _controller.GetPurchasesForThePastYear().ToList();
            
            CollectionAssert.AreEqual(expectedPurchases, result);
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public void GetPurchaseByCorrectId_ShouldReturnCorrectPurchase()
        {
            var currentUserId = _userIds.First();
            var purchaseToGet = _purchases.Where(p => p.UserId == currentUserId).First();

            var purchaseRetrieved = _controller.GetPurchaseById(purchaseToGet.Id);

            Assert.IsInstanceOfType(purchaseRetrieved, typeof(OkNegotiatedContentResult<PurchaseDto>));
            var contentNegResult = (OkNegotiatedContentResult<PurchaseDto>)purchaseRetrieved;

            Assert.AreEqual(purchaseToGet.ToDto(), contentNegResult.Content);
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public void GetPurchaseByIncorrectId_ShouldReturnNotFound()
        {
            var incorrectPurchaseId = Guid.NewGuid();

            var purchaseRetrieved = _controller.GetPurchaseById(incorrectPurchaseId);

            Assert.IsInstanceOfType(purchaseRetrieved, typeof(NotFoundResult));
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public async Task MakePurchase_ShouldReturnSuccessfulPurchase()
        {
            var itemsToPurchase = _items.Take(10);
            var purchaseRequest = new PurchaseRequest();
            purchaseRequest.Items = itemsToPurchase
                .Select(i => new PurchaseRequestItem()
                {
                    ItemId = i.Id,
                    Quantity = 1
                }).ToList();

            var expectedPurchasedItemDtos = itemsToPurchase.Select(i => new PurchasedItemDto()
            {
                Item = i.ToDto(),
                Quantity = 1,
                UnitPrice = i.Price
            }).ToList();
            purchaseRequest.PaymentInfo = new PaymentInfo()
            {
                Card = CreditCard.Visa,
                CardHolderName = "John Doe",
                CardNumber = 1234567891234567,
                CardSecurityCode = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2017,
                BillingAddress = "Billing Address"
            };

            var purchaseResult = await _controller.MakePurchase(purchaseRequest);

            Assert.IsInstanceOfType(purchaseResult, typeof(CreatedAtRouteNegotiatedContentResult<PurchaseDto>));
            var contentNegResult = (CreatedAtRouteNegotiatedContentResult<PurchaseDto>)purchaseResult;

            // Set the purchaseId of the purchasedItems before asserting equality of expected vs returned purchase info
            expectedPurchasedItemDtos.ForEach(p => p.PurchaseId = contentNegResult.Content.Id);

            CollectionAssert.AreEqual(expectedPurchasedItemDtos, contentNegResult.Content.PurchasedItems.ToList());
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public async Task MakePurchase_WithNotEnoughStock_ShouldReturnBadRequest()
        {
            _purchaseService.When(x => x.ValidatePurchaseRequest(Arg.Any<IList<PurchaseRequestItem>>()))
                .Throw(new ArgumentException("Insufficiently stocked purchase request items received."));

            var itemsToPurchase = _items.Take(10);
            var purchaseRequest = new PurchaseRequest();
            purchaseRequest.Items = itemsToPurchase
                .Select(i => new PurchaseRequestItem()
                {
                    ItemId = i.Id,
                    Quantity = 100
                }).ToList();
            
            purchaseRequest.PaymentInfo = new PaymentInfo()
            {
                Card = CreditCard.Visa,
                CardHolderName = "John Doe",
                CardNumber = 1234567891234567,
                CardSecurityCode = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2017,
                BillingAddress = "Billing Address"
            };

            var purchaseResult = await _controller.MakePurchase(purchaseRequest);

            Assert.IsInstanceOfType(purchaseResult, typeof(BadRequestErrorMessageResult));
            Assert.AreEqual(((BadRequestErrorMessageResult)purchaseResult).Message, "Insufficiently stocked purchase request items received.");
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public async Task MakePurchase_WithNoItems_ShouldReturnBadRequest()
        {
            _purchaseService.When(x => x.ValidatePurchaseRequest(Arg.Any<IList<PurchaseRequestItem>>()))
                .Throw(new ArgumentException("No purchase request items received."));
            
            var purchaseRequest = new PurchaseRequest();
            purchaseRequest.Items = new List<PurchaseRequestItem>();
            purchaseRequest.PaymentInfo = new PaymentInfo()
            {
                Card = CreditCard.Visa,
                CardHolderName = "John Doe",
                CardNumber = 1234567891234567,
                CardSecurityCode = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2017,
                BillingAddress = "Billing Address"
            };

            var purchaseResult = await _controller.MakePurchase(purchaseRequest);

            Assert.IsInstanceOfType(purchaseResult, typeof(BadRequestErrorMessageResult));
            Assert.AreEqual(((BadRequestErrorMessageResult)purchaseResult).Message, "No purchase request items received.");
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public async Task MakePurchase_WithNonExistentItems_ShouldReturnBadRequest()
        {
            Guid invalidItemId = Guid.NewGuid();

            _purchaseService.When(x => x.ValidatePurchaseRequest(Arg.Any<IList<PurchaseRequestItem>>()))
                .Throw(new ArgumentException("Unrecognized purchase request item(s) received: " + invalidItemId.ToString()));
            
            var purchaseRequest = new PurchaseRequest();
            purchaseRequest.Items = new List<PurchaseRequestItem>()
            {
                new PurchaseRequestItem()
                {
                    ItemId = invalidItemId,
                    Quantity = 1
                }
            };
            purchaseRequest.PaymentInfo = new PaymentInfo()
            {
                Card = CreditCard.Visa,
                CardHolderName = "John Doe",
                CardNumber = 1234567891234567,
                CardSecurityCode = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2017,
                BillingAddress = "Billing Address"
            };

            var purchaseResult = await _controller.MakePurchase(purchaseRequest);

            Assert.IsInstanceOfType(purchaseResult, typeof(BadRequestErrorMessageResult));
            Assert.AreEqual(((BadRequestErrorMessageResult)purchaseResult).Message, "Unrecognized purchase request item(s) received: " + invalidItemId.ToString());
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public async Task MakePurchase_WithUnsuccessfulPayment_ShouldReturnBadRequest()
        {
            _paymentService.ProcessPayment(Arg.Any<decimal>(), Arg.Any<PaymentInfo>())
                .Returns(p => new PaymentProcessResult() { Success = false, ErrorMessage = "Credit card declined." });

            var itemsToPurchase = _items.Take(10);
            var purchaseRequest = new PurchaseRequest();
            purchaseRequest.Items = itemsToPurchase
                .Select(i => new PurchaseRequestItem()
                {
                    ItemId = i.Id,
                    Quantity = 1
                }).ToList();

            var expectedPurchasedItemDtos = itemsToPurchase.Select(i => new PurchasedItemDto()
            {
                Item = i.ToDto(),
                Quantity = 1,
                UnitPrice = i.Price
            }).ToList();
            purchaseRequest.PaymentInfo = new PaymentInfo()
            {
                Card = CreditCard.Visa,
                CardHolderName = "John Doe",
                CardNumber = 1234567891234567,
                CardSecurityCode = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2017,
                BillingAddress = "Billing Address"
            };

            var purchaseResult = await _controller.MakePurchase(purchaseRequest);

            Assert.IsInstanceOfType(purchaseResult, typeof(BadRequestErrorMessageResult));
            Assert.AreEqual(((BadRequestErrorMessageResult)purchaseResult).Message, "Payment failure: Credit card declined.");
        }

        protected override void SetupData()
        {
            _userIds = new List<string>();
            _userIds.Add(Guid.NewGuid().ToString());
            _userIds.Add(Guid.NewGuid().ToString());

            _purchases = new List<Purchase>();
            Guid purchaseId1 = Guid.NewGuid();
            Guid purchaseId2 = Guid.NewGuid();
            Guid purchaseId3 = Guid.NewGuid();
            _purchases.Add(new Purchase()
            {
                UserId = _userIds[0],
                Id = purchaseId1,
                IsReturn = false,
                Date = DateTime.UtcNow,
                PurchasedItems = new List<Item>() { _items[0], _items[4], _items[7] }
                    .Select(i => new PurchasedItem()
                    {
                        Id = Guid.NewGuid(),
                        Item = i,
                        ItemId = i.Id,
                        PurchaseId = purchaseId1,
                        Quantity = 2,
                        UnitPrice = i.Price
                    }).ToList()
            });
            _purchases.Add(new Purchase()
            {
                UserId = _userIds[0],
                Id = purchaseId2,
                IsReturn = false,
                Date = DateTime.UtcNow.AddMonths(-13), // Over a year ago
                PurchasedItems = new List<Item>() { _items[1], _items[5], _items[8] }
                    .Select(i => new PurchasedItem()
                    {
                        Id = Guid.NewGuid(),
                        Item = i,
                        ItemId = i.Id,
                        PurchaseId = purchaseId2,
                        Quantity = 1,
                        UnitPrice = i.Price
                    }).ToList()
            });
            _purchases.Add(new Purchase()
            {
                UserId = _userIds[1],
                Id = purchaseId3,
                IsReturn = false,
                Date = DateTime.UtcNow,
                PurchasedItems = new List<Item>() { _items[16] }
                    .Select(i => new PurchasedItem()
                    {
                        Id = Guid.NewGuid(),
                        Item = i,
                        ItemId = i.Id,
                        PurchaseId = purchaseId3,
                        Quantity = 1,
                        UnitPrice = i.Price
                    }).ToList()
            });
        }
        protected override void SetupClass()
        {
            _purchaseService = Substitute.For<IPurchaseService>();

            _purchaseService
                .GetPurchaseById(Arg.Any<string>(), Arg.Any<Guid>())
                .Returns(g => _purchases.FirstOrDefault(p => p.UserId == (string)g[0] && p.Id == (Guid)g[1]));

            _purchaseService
                .GetPurchases(Arg.Any<string>(), Arg.Any<DateTime>())
                .Returns(g => _purchases.Where(p => p.UserId == (string)g[0] && p.Date > (DateTime)g[1]));

            _purchaseService
                .AddPurchase(Arg.Any<string>(), Arg.Any<IList<PurchaseRequestItem>>())
                .Returns(a => {
                    Guid purchaseId = Guid.NewGuid();
                    return new Purchase()
                    {
                        Date = DateTime.UtcNow,
                        Id = purchaseId,
                        IsReturn = false,
                        UserId = (string)a[0],
                        PurchasedItems = ((IList<PurchaseRequestItem>)a[1])
                            .Select(p => new PurchasedItem()
                            {
                                Id = Guid.NewGuid(),
                                Item = _items.Single(i => i.Id == p.ItemId),
                                ItemId = p.ItemId,
                                PurchaseId = purchaseId,
                                Quantity = p.Quantity,
                                UnitPrice = _items.Single(i => i.Id == p.ItemId).Price
                        }).ToList()
                    };
                });

            _paymentService = Substitute.For<IPaymentService>();

            _paymentService.ProcessPayment(Arg.Any<decimal>(), Arg.Any<PaymentInfo>())
                .Returns(new PaymentProcessResult()
                {
                    Success = true,
                    ConfirmationCode = Guid.NewGuid().ToString()
                });
            
            var itemService = Substitute.For<IItemService>();

            itemService
                .GetItems()
                .Returns(_items);

            itemService
                .GetItemById(Arg.Any<Guid>())
                .Returns(g => _items.FirstOrDefault(i => i.Id == (Guid)g[0]));

            itemService
                .GetItemsByCategory(Arg.Any<Guid>())
                .Returns(g => _items.Where(i => i.CategoryId == (Guid)g[0]));

            itemService
                .GetItemStockLevels()
                .Returns(g => _items.ToDictionary(k => k.Id, v => v.Stock));

            itemService
                .When(x => x.UpdateStockLevel(Arg.Any<Guid>(), Arg.Any<int>()))
                .Do(x => _items.First(i => i.Id == (Guid)x[0]).Stock = (int)x[1]);

            itemService
                .GetTotalPrice(Arg.Any<IList<PurchaseRequestItem>>())
                .Returns(g => ((IList<PurchaseRequestItem>)g[0]).Sum(r => _items.FirstOrDefault(i => i.Id == r.ItemId).Price * r.Quantity));

            var userService = Substitute.For<IUserService>();

            userService
                .GetCurrentUserId()
                .Returns(_userIds.First());

            _controller = new PurchaseController(_purchaseService, _paymentService, itemService, userService);
        }
    }
}
