using GildedRose.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GildedRose.EFDataProvider
{
    public class PriceOverrideMapping : EntityTypeConfiguration<PriceOverride>
    {
        /// <summary>
        /// Mapping for the PriceOverride entity.
        /// </summary>
        public PriceOverrideMapping()
        {
            HasKey(tn => new { tn.Id });

            Property(tn => tn.Id).HasColumnName(DatabaseConstants.Id).IsRequired();
            Property(tn => tn.Price).HasColumnName(DatabaseConstants.Price).IsRequired();
            Property(tn => tn.StartDate).HasColumnName(DatabaseConstants.StartDate).IsRequired();
            Property(tn => tn.EndDate).HasColumnName(DatabaseConstants.EndDate).IsRequired();

            ToTable(DatabaseConstants.PriceOverride, DatabaseConstants.DefaultSchema);
        }
    }
}