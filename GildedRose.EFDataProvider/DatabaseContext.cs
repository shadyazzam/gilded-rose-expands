using GildedRose.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace GildedRose.EFDataProvider
{
    /// <summary>
    /// Entity Framework database context
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<PurchasedItem> PurchasedItems { get; set; }
        public virtual DbSet<PriceOverride> PriceOverrides { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DatabaseContext(string connectionString) : base(connectionString)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        public DatabaseContext() : base("name=GildedRoseSqlServer")
        {
            Initialize();
        }

        protected internal void Initialize()
        {
            // Entity Framework 6 throws an exception unless we first probe the provider types.
            var type = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context. 
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Add all of the Mapping configurations from this assembly.
            modelBuilder.Configurations.AddFromAssembly(typeof(DatabaseContext).Assembly);
        }
    }
}
