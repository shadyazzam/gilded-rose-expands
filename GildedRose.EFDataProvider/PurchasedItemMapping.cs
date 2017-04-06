using GildedRose.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GildedRose.EFDataProvider
{
    public class PurchasedItemMapping : EntityTypeConfiguration<PurchasedItem>
    {
        /// <summary>
        /// Mapping for the PurchasedItem entity.
        /// </summary>
        public PurchasedItemMapping()
        {
            HasKey(tn => new { tn.Id });

            Property(tn => tn.Id).HasColumnName(DatabaseConstants.Id).IsRequired();
            Property(tn => tn.PurchaseId).HasColumnName(DatabaseConstants.PurchaseId).IsRequired();
            Property(tn => tn.ItemId).HasColumnName(DatabaseConstants.ItemId).IsRequired();
            Property(tn => tn.Quantity).HasColumnName(DatabaseConstants.Quantity).IsRequired();
            Property(tn => tn.UnitPrice).HasColumnName(DatabaseConstants.UnitPrice).IsRequired();

            HasRequired(i => i.Item)
                .WithMany()
                .HasForeignKey(i => i.ItemId);

            ToTable(DatabaseConstants.PurchasedItem, DatabaseConstants.DefaultSchema);
        }
    }
}