using GildedRose.Entities;
using GildedRose.Logic;
using GildedRose.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Tests.Unit.Services
{
    [TestClass]
    public class PurchaseServiceTest : BaseTest
    {
        // Dependencies
        List<string> _userIds;
        List<Purchase> _purchases;
        IPurchaseService _service;
        IItemService _itemService;
        IRepository<Purchase> _purchaseRepository;
        DateTime oneYearAgo = DateTime.UtcNow.AddYears(-1);
        
        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetPurchases_ShouldReturnAllUserPurchasesForLastYear()
        {
            var currentUserId = _userIds.First();
            var result = _service.GetPurchases(currentUserId, oneYearAgo).ToList();
            
            var expectedPurchases = _purchases.Where(p => p.UserId == _userIds.First() && p.Date > oneYearAgo).ToList();

            CollectionAssert.AreEqual(expectedPurchases, result);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetPurchaseByCorrectId_ShouldReturnCorrectPurchase()
        {
            var currentUserId = _userIds.First();
            var purchaseToGet = _purchases.Where(p => p.UserId == currentUserId).First();

            var purchaseRetrieved = _service.GetPurchaseById(currentUserId, purchaseToGet.Id);
            
            Assert.AreEqual(purchaseToGet, purchaseRetrieved);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetPurchaseByIncorrectId_ShouldReturnNotFound()
        {
            var currentUserId = _userIds.First();
            var incorrectPurchaseId = Guid.NewGuid();

            var purchaseRetrieved = _service.GetPurchaseById(currentUserId, incorrectPurchaseId);

            Assert.IsNull(purchaseRetrieved);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void MakePurchase_ShouldReturnSuccessfulPurchase()
        {
            var currentUserId = _userIds.First();
            var itemsToPurchase = _items.Take(10);

            // Keep track of current stock levels
            var stockLevels = itemsToPurchase.ToDictionary(k => k.Id, v => v.Stock);

            var purchaseRequestItems = itemsToPurchase
                .Select(i => new PurchaseRequestItem()
                {
                    ItemId = i.Id,
                    Quantity = 1
                }).ToList();

            var expectedPurchasedItems = purchaseRequestItems
                .Select(i => new PurchasedItem()
                {
                    Item = _items.Single(it => it.Id == i.ItemId),
                    ItemId = i.ItemId,
                    Quantity = i.Quantity,
                    UnitPrice = _items.Single(it => it.Id == i.ItemId).Price,
                }).ToList();
            
            var purchaseResult = _service.AddPurchase(currentUserId, purchaseRequestItems);

            // Set the purchaseId of the purchasedItems before asserting equality of expected vs returned purchase info
            expectedPurchasedItems.ForEach(p =>
            {
                p.PurchaseId = purchaseResult.Id;
                p.Id = purchaseResult.PurchasedItems.Single(s => s.ItemId == p.ItemId).Id;
            });

            CollectionAssert.AreEqual(expectedPurchasedItems, purchaseResult.PurchasedItems.ToList());

            // Verify stock levels decremented
            var expectedStockLevels = stockLevels.ToDictionary(k => k.Key, v => v.Value - 1);

            var currentStockLevels = _itemService.GetItemStockLevels().ToDictionary(k => k.Key, v => v.Value);

            CollectionAssert.IsSubsetOf(expectedStockLevels, currentStockLevels);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        [ExpectedException(typeof(ArgumentException), "No purchase request items received.")]
        public void MakePurchase_WithZeroQuantities_ShouldThrowException()
        {
            var currentUserId = _userIds.First();
            var itemsToPurchase = _items.Take(10);

            var purchaseRequestItems = itemsToPurchase
                .Select(i => new PurchaseRequestItem()
                {
                    ItemId = i.Id,
                    Quantity = 0
                }).ToList();

            var purchaseResult = _service.AddPurchase(currentUserId, purchaseRequestItems);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        [ExpectedException(typeof(ArgumentException), "No purchase request items received.")]
        public void MakePurchase_WithNoItems_ShouldThrowException()
        {
            var currentUserId = _userIds.First();
            var purchaseRequestItems = new List<PurchaseRequestItem>();

            var purchaseResult = _service.AddPurchase(currentUserId, purchaseRequestItems);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        [ExpectedException(typeof(ArgumentNullException), "No purchase request items received.")]
        public void MakePurchase_WithNullList_ShouldThrowException()
        {
            var currentUserId = _userIds.First();
            var purchaseResult = _service.AddPurchase(currentUserId, null);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        [ExpectedException(typeof(ArgumentException), "No purchase request items received.")]
        public void MakePurchase_WithNullItems_ShouldThrowException()
        {
            var currentUserId = _userIds.First();
            var purchaseRequestItems = new List<PurchaseRequestItem>()
            {
                null, null, null
            };

            var purchaseResult = _service.AddPurchase(currentUserId, purchaseRequestItems);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        [ExpectedException(typeof(ArgumentException), "Unrecognized purchase request item(s) received: ")]
        public void MakePurchase_WithNonExistentItems_ShouldThrowException()
        {
            var currentUserId = _userIds.First();
            var purchaseRequestItems = new List<PurchaseRequestItem>()
            {
                new PurchaseRequestItem() { ItemId = Guid.NewGuid(), Quantity = 1 },
                new PurchaseRequestItem() { ItemId = Guid.NewGuid(), Quantity = 2 }
            };

            var purchaseResult = _service.AddPurchase(currentUserId, purchaseRequestItems);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Insufficiently stocked purchase request item(s) received:")]
        public void MakePurchase_OutOfStock_ShouldThrowException()
        {
            var currentUserId = _userIds.First();
            var purchaseRequestItems = new List<PurchaseRequestItem>()
            {
                new PurchaseRequestItem() { ItemId = _items[0].Id, Quantity = 1000 }, // Quantity > Stock
                new PurchaseRequestItem() { ItemId = _items[2].Id, Quantity = 2 }
            };

            var purchaseResult = _service.AddPurchase(currentUserId, purchaseRequestItems);
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
            _purchaseRepository = Substitute.For<IRepository<Purchase>>();
            
            _purchaseRepository
                .Get(Arg.Any<Guid>())
                .Returns(g => _purchases.FirstOrDefault(i => i.Id == (Guid)g[0]));

            _purchaseRepository
                .Save();
            
            _purchaseRepository
                .Get(Arg.Any<Expression<Func<Purchase, bool>>>(), Arg.Any<Func<IQueryable<Purchase>, IOrderedQueryable<Purchase>>>())
                .Returns(g =>
                {
                    var binaryExpression = (BinaryExpression)((LambdaExpression)g.Args()[0]).Body;

                    var value = ((ConstantExpression)((MemberExpression)((BinaryExpression)binaryExpression.Right).Right).Expression).Value;

                    if (value.GetType().GetField("fromDate") != null)
                    {
                        var fromDate = (DateTime)value.GetType().GetField("fromDate").GetValue(value);
                        var userId = (string)value.GetType().GetField("userId").GetValue(value);

                        return _purchases.Where(p => p.UserId == userId && p.Date > fromDate);
                    }
                    else
                    {
                        var purchaseId = (Guid)value.GetType().GetField("purchaseId").GetValue(value);
                        var userId = (string)value.GetType().GetField("userId").GetValue(value);

                        //return _purchases.Where(p => p.UserId == userId && p.Date > fromDate);
                        return _purchases.Where(p => p.UserId == userId && p.Id == purchaseId);
                    }
                });

            _purchaseRepository
                .When(x => x.Create(Arg.Any<Purchase>()))
                .Do(d => _purchases.Add((Purchase)d[0]));

            var itemRepository = Substitute.For<IRepository<Item>>();

            itemRepository
                .Get()
                .Returns(_items);

            itemRepository
                .Get(Arg.Any<Guid>())
                .Returns(g => _items.FirstOrDefault(i => i.Id == (Guid)g[0]));

            itemRepository
                .Save();

            _itemService = new ItemService(itemRepository);

            _service = new PurchaseService(_purchaseRepository, _itemService);
        }
    }
}
