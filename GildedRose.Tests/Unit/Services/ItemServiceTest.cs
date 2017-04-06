using GildedRose.Entities;
using GildedRose.Logic;
using GildedRose.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Tests.Unit.Services
{
    [TestClass]
    public class ItemServiceTest : BaseTest
    {
        // Dependencies
        IItemService _service;
        
        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetItems_ShouldReturnAllItems()
        {
            var result = _service.GetItems().ToList();

            CollectionAssert.AreEqual(_items, result);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetItem_ByCorrectId_ShouldReturnCorrectItem()
        {
            var expectedItem = _items.First();
            var result = _service.GetItemById(expectedItem.Id);

            Assert.AreEqual(expectedItem, result);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetItem_ByIncorrectId_ShouldReturnNull()
        {
            var result = _service.GetItemById(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetItems_ByExistingCategoryId_ShouldReturnItemsForCategory()
        {
            var category = _categories.First();
            var expectedItems = _items.Where(i => i.CategoryId == category.Id);

            var result = _service.GetItemsByCategory(category.Id);

            CollectionAssert.AreEqual(expectedItems.ToList(), result.ToList());
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetItems_ByNonExistingCategoryId_ShouldReturnEmpty()
        {
            var result = _service.GetItemsByCategory(Guid.NewGuid());

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetItemStockLevels_ShouldReturnCorrectStockLevels()
        {
            var expectedStockLevels = _items.ToDictionary(k => k.Id, v => v.Stock);

            var stockLevels = _service.GetItemStockLevels();

            Assert.IsTrue(expectedStockLevels.ContentEquals(stockLevels));
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetTotalPrice_ShouldReturnCorrectTotalPrice()
        {
            int quantityPerItem = 3;
            var items = _items.Take(10);
            decimal expectedTotalPrice = items.Sum(i => i.Price * quantityPerItem);

            decimal totalPrice = _service.GetTotalPrice(items.Select(i => new PurchaseRequestItem()
            {
                ItemId = i.Id,
                Quantity = quantityPerItem
            }).ToList());

            Assert.AreEqual(expectedTotalPrice, totalPrice);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void UpdateStockLevel_ShouldUpdateItemStock()
        {
            var item = _items.First();

            _service.UpdateStockLevel(item.Id, 1234);

            Assert.AreEqual(item.Stock, 1234);
        }

        protected override void SetupData()
        {
            // No further data to setup here.
        }
        protected override void SetupClass()
        {
            var itemRepository = Substitute.For<IRepository<Item>>();
            
            itemRepository
                .Get(Arg.Any<Guid>())
                .Returns(g => _items.FirstOrDefault(i => i.Id == (Guid)g[0]));

            itemRepository
                .Save();

            itemRepository
                .Get(Arg.Any<Expression<Func<Item, bool>>>(), Arg.Any<Func<IQueryable<Item>, IOrderedQueryable<Item>>>())
                .Returns(g =>
                {
                    if (g.Args()[0] == null)
                        return _items;

                    var binaryExpression = (BinaryExpression)((LambdaExpression)g.Args()[0]).Body;

                    var value = ((ConstantExpression)((MemberExpression)binaryExpression.Right).Expression).Value;

                    var categoryId = (Guid)value.GetType().GetField("categoryId").GetValue(value);

                    return _items.Where(i => i.CategoryId == categoryId);
                });

            _service = new ItemService(itemRepository);
        }
    }

    public static class DictionaryExtensionMethods
    {
        public static bool ContentEquals<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> otherDictionary)
        {
            return (otherDictionary ?? new Dictionary<TKey, TValue>())
                .OrderBy(kvp => kvp.Key)
                .SequenceEqual((dictionary ?? new Dictionary<TKey, TValue>())
                                   .OrderBy(kvp => kvp.Key));
        }
    }
}
