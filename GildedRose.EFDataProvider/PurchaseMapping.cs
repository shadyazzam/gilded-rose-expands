using GildedRose.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GildedRose.EFDataProvider
{
    public class PurchaseMapping : EntityTypeConfiguration<Purchase>
    {
        /// <summary>
        /// Mapping for the Purchase entity.
        /// </summary>
        public PurchaseMapping()
        {
            HasKey(tn => new { tn.Id });

            Property(tn => tn.Id).HasColumnName(DatabaseConstants.Id).IsRequired();
            Property(tn => tn.UserId).HasColumnName(DatabaseConstants.UserId).IsRequired();
            Property(tn => tn.Date).HasColumnName(DatabaseConstants.Date).IsRequired();
            Property(tn => tn.IsReturn).HasColumnName(DatabaseConstants.IsReturn).IsRequired();

            HasMany(p => p.PurchasedItems)
                .WithRequired()
                .HasForeignKey(i => i.PurchaseId);

            ToTable(DatabaseConstants.Purchase, DatabaseConstants.DefaultSchema);
        }
    }
}