using GildedRose.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GildedRose.EFDataProvider
{
    public class ItemMapping : EntityTypeConfiguration<Item>
    {
        /// <summary>
        /// Mapping for the Item entity.
        /// </summary>
        public ItemMapping()
        {
            HasKey(i => new { i.Id });

            Property(i => i.Id).HasColumnName(DatabaseConstants.Id).IsRequired();
            Property(i => i.Name).HasColumnName(DatabaseConstants.Name).IsRequired();
            Property(i => i.Description).HasColumnName(DatabaseConstants.Description).IsRequired();
            Property(i => i.Price).HasColumnName(DatabaseConstants.Price).IsRequired();
            Property(i => i.CategoryId).HasColumnName(DatabaseConstants.CategoryId).IsRequired();
            Property(i => i.Stock).HasColumnName(DatabaseConstants.Stock).IsRequired();

            HasRequired(i => i.Category)
                .WithMany()
                .HasForeignKey(i => i.CategoryId);

            ToTable(DatabaseConstants.Item, DatabaseConstants.DefaultSchema);
        }
    }
}