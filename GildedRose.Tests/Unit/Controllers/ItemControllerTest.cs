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

namespace GildedRose.Tests.Unit.Controllers
{
    [TestClass]
    public class ItemControllerTest : BaseTest
    {
        // Dependencies
        ItemController _controller;
        
        [TestMethod]
        [TestCategory("ControllerTests")]
        public void GetItems_ShouldReturnAllItems()
        {
            var expectedItems = _items.Select(i => i.ToDto()).ToList();

            var result = _controller.GetItems().ToList();            

            CollectionAssert.AreEqual(expectedItems, result);
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public void GetItemByCorrectId_ShouldReturnCorrectItem()
        {
            var itemToGet = _items.First();

            var itemRetrieved = _controller.GetItemById(itemToGet.Id);
            
            Assert.IsInstanceOfType(itemRetrieved, typeof(OkNegotiatedContentResult<ItemDto>));
            var contentNegResult = (OkNegotiatedContentResult<ItemDto>)itemRetrieved;

            Assert.AreEqual(itemToGet.ToDto(), contentNegResult.Content);
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public void GetItemByIncorrectId_ShouldReturnNotFound()
        {
            var incorrectItemId = Guid.NewGuid();

            var itemRetrieved = _controller.GetItemById(incorrectItemId);

            Assert.IsInstanceOfType(itemRetrieved, typeof(NotFoundResult));
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public void GetItemsByValidCategoryId_ShouldReturnItemsForCategory()
        {
            var category = _categories.First();
            var itemsToGet = _items.Where(i => i.CategoryId == category.Id);
            var expectedItems = itemsToGet.Select(i => i.ToDto()).ToList();

            var itemsRetrieved = _controller.GetItemsByCategoryId(category.Id).ToList();
            
            CollectionAssert.AreEqual(expectedItems, itemsRetrieved);
        }

        [TestMethod]
        [TestCategory("ControllerTests")]
        public void GetItemsByInvalidCategoryId_ShouldReturnEmpty()
        {
            var invalidCategoryId = Guid.NewGuid();

            var itemsRetrieved = _controller.GetItemsByCategoryId(invalidCategoryId);

            Assert.IsFalse(itemsRetrieved.Any());
        }

        protected override void SetupData()
        {
            // No further data to setup here.
        }
        protected override void SetupClass()
        {
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
            
            _controller = new ItemController(itemService);
        }
    }
}
