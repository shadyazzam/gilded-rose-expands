using GildedRose.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GildedRose.Tests.Unit
{
    [TestClass]
    public abstract class BaseTest
    {
        protected abstract void SetupData();
        protected abstract void SetupClass();

        [TestInitialize]
        public void Initialize()
        {
            SetupCommonData();
            SetupClass();
        }

        protected List<Category> _categories;
        protected List<Item> _items;


        private void SetupCommonData()
        {
            Guid officeCategoryId = Guid.NewGuid();
            Guid kitchenCategoryId = Guid.NewGuid();
            Guid furnitureCategoryId = Guid.NewGuid();

            Category officeCategory = new Category() { Id = officeCategoryId, Name = "Office" };
            Category kitchenCategory = new Category() { Id = kitchenCategoryId, Name = "Kitchen" };
            Category furnitureCategory = new Category() { Id = furnitureCategoryId, Name = "Furniture" };
            _categories = new List<Category>();
            _categories.Add(officeCategory);
            _categories.Add(kitchenCategory);
            _categories.Add(furnitureCategory);

            _items = new List<Item>();
            _items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = officeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Desk",
                Description = "Office Desk",
                Price = 325,
                Stock = 18
            });
            _items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = officeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Pen",
                Description = "Fountain Pen",
                Price = 9,
                Stock = 95
            });
            _items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = officeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Stapler",
                Description = "Stapler",
                Price = 5,
                Stock = 55
            });
            _items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = officeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Notebook",
                Description = "Notebook",
                Price = 20,
                Stock = 47
            });
            _items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = kitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Cups",
                Description = "Cup Set",
                Price = 7,
                Stock = 25
            });
            _items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = kitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Cuttlery",
                Description = "Cuttlery",
                Price = 5,
                Stock = 30
            });
            _items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = kitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Knife Block",
                Description = "Knife Block",
                Price = 20,
                Stock = 15
            });
            _items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = officeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Plates",
                Description = "Plate Set",
                Price = 15,
                Stock = 10
            });
            _items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = kitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Blender",
                Description = "Blender",
                Price = 40,
                Stock = 10
            });
            _items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = kitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Cutting Board",
                Description = "Cutting Board",
                Price = 10,
                Stock = 20
            });
            _items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = furnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Couch",
                Description = "Couch",
                Price = 750,
                Stock = 3
            });
            _items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = furnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Ceiling Fan",
                Description = "Ceiling Fan",
                Price = 135,
                Stock = 12
            });
            _items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = furnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Dining Room Table",
                Description = "Dining Room Table",
                Price = 635,
                Stock = 7
            });
            _items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = furnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Lamp",
                Description = "Lamp",
                Price = 40,
                Stock = 25
            });
            _items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = furnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Shelf",
                Description = "Shelf",
                Price = 175,
                Stock = 25
            });
            _items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = furnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Coffee Table",
                Description = "Coffee Table",
                Price = 25,
                Stock = 10
            });
            _items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = furnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Bed",
                Description = "Bed",
                Price = 250,
                Stock = 5
            });

            // Call the child class' SetupData() to setup more data specific to the class
            SetupData();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // TODO:
        }
    }
}
