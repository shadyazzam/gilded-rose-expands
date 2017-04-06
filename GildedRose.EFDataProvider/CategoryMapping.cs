using GildedRose.Entities;
using System.Data.Entity.ModelConfiguration;

namespace GildedRose.EFDataProvider
{
    /// <summary>
    /// Mapping for the Category entity.
    /// </summary>
    public class CategoryMapping : EntityTypeConfiguration<Category>
    {
        public CategoryMapping()
        {
            HasKey(tn => new { tn.Id });

            Property(tn => tn.Id).HasColumnName(DatabaseConstants.Id).IsRequired();
            Property(tn => tn.Name).HasColumnName(DatabaseConstants.Name).IsRequired();

            ToTable(DatabaseConstants.Category, DatabaseConstants.DefaultSchema);
        }
    }
}