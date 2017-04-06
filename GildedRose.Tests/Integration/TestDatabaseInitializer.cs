using GildedRose.EFDataProvider;
using GildedRose.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GildedRose.Tests.Integration
{
    /// <summary>
    /// This class is used for testing in order to drop, create, and seed the database for testing purposes.
    /// </summary>
    /// <seealso cref="DatabaseContext" />
    public class TestDatabaseInitializer : DropCreateDatabaseAlways<DatabaseContext>
    {
        public string DbName { get; set; }

        public Guid OfficeCategoryId { get; set; }
        public Guid KitchenCategoryId { get; set; }
        public Guid FurnitureCategoryId { get; set; }
        
        public List<Item> Items { get; set; }

        public List<Category> Categories { get; set; }

        public TestDatabaseInitializer()
        {
            CreateTestData();
        }

        protected override void Seed(DatabaseContext context)
        {
            context.Categories.AddRange(Categories);
            context.Items.AddRange(Items);

            context.SaveChanges();

            DbName = context.Database.Connection.Database;
        }

        private void CreateTestData()
        {
            // Categories
            Categories = new List<Category>();

            OfficeCategoryId = Guid.NewGuid();
            KitchenCategoryId = Guid.NewGuid();
            FurnitureCategoryId = Guid.NewGuid();

            Category officeCategory = new Category() { Id = OfficeCategoryId, Name = "Office" };
            Category kitchenCategory = new Category() { Id = KitchenCategoryId, Name = "Kitchen" };
            Category furnitureCategory = new Category() { Id = FurnitureCategoryId, Name = "Furniture" };

            Categories.Add(officeCategory);
            Categories.Add(kitchenCategory);
            Categories.Add(furnitureCategory);

            // Items
            Items = new List<Item>();
            
            Items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = OfficeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Desk",
                Description = "Office Desk",
                Price = 325,
                Stock = 18
            });
            Items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = OfficeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Pen",
                Description = "Fountain Pen",
                Price = 9,
                Stock = 95
            });
            Items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = OfficeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Stapler",
                Description = "Stapler",
                Price = 5,
                Stock = 55
            });
            Items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = OfficeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Notebook",
                Description = "Notebook",
                Price = 20,
                Stock = 47
            });
            Items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = KitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Cups",
                Description = "Cup Set",
                Price = 7,
                Stock = 25
            });
            Items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = KitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Cuttlery",
                Description = "Cuttlery",
                Price = 5,
                Stock = 30
            });
            Items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = KitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Knife Block",
                Description = "Knife Block",
                Price = 20,
                Stock = 15
            });
            Items.Add(new Item()
            {
                Category = officeCategory,
                CategoryId = OfficeCategoryId,
                Id = Guid.NewGuid(),
                Name = "Plates",
                Description = "Plate Set",
                Price = 15,
                Stock = 10
            });
            Items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = KitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Blender",
                Description = "Blender",
                Price = 40,
                Stock = 10
            });
            Items.Add(new Item()
            {
                Category = kitchenCategory,
                CategoryId = KitchenCategoryId,
                Id = Guid.NewGuid(),
                Name = "Cutting Board",
                Description = "Cutting Board",
                Price = 10,
                Stock = 20
            });
            Items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = FurnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Couch",
                Description = "Couch",
                Price = 750,
                Stock = 3
            });
            Items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = FurnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Ceiling Fan",
                Description = "Ceiling Fan",
                Price = 135,
                Stock = 12
            });
            Items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = FurnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Dining Room Table",
                Description = "Dining Room Table",
                Price = 635,
                Stock = 7
            });
            Items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = FurnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Lamp",
                Description = "Lamp",
                Price = 40,
                Stock = 25
            });
            Items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = FurnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Shelf",
                Description = "Shelf",
                Price = 175,
                Stock = 25
            });
            Items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = FurnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Coffee Table",
                Description = "Coffee Table",
                Price = 25,
                Stock = 10
            });
            Items.Add(new Item()
            {
                Category = furnitureCategory,
                CategoryId = FurnitureCategoryId,
                Id = Guid.NewGuid(),
                Name = "Bed",
                Description = "Bed",
                Price = 250,
                Stock = 5
            });
        }
    }
}