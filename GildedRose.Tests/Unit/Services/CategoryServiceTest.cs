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
    public class CategoryServiceTest : BaseTest
    {
        // Dependencies
        ICategoryService _service;
        
        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetCategories_ShouldReturnAllCategories()
        {
            var result = _service.GetCategories().ToList();

            CollectionAssert.AreEqual(_categories, result);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetCategory_ByCorrectId_ShouldReturnCorrectCategory()
        {
            var expectedCategory = _categories.First();
            var result = _service.GetCategoryById(expectedCategory.Id);

            Assert.AreEqual(expectedCategory, result);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void GetCategory_ByIncorrectId_ShouldReturnNull()
        {
            var result = _service.GetCategoryById(Guid.NewGuid());

            Assert.IsNull(result);
        }

        protected override void SetupData()
        {
            // No further data to setup here.
        }
        protected override void SetupClass()
        {
            var categoryRepository = Substitute.For<IRepository<Category>>();

            categoryRepository
                .Get()
                .Returns(_categories);

            categoryRepository
                .Get(Arg.Any<Guid>())
                .Returns(g => _categories.FirstOrDefault(i => i.Id == (Guid)g[0]));

            categoryRepository
                .Save();

            _service = new CategoryService(categoryRepository);
        }
    }
}
