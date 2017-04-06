namespace GildedRose.EFDataProvider
{
    /// <summary>
    /// Contains strings related to database access.
    /// </summary>
    public static class DatabaseConstants
    {
        // Schema Names
        public const string DefaultSchema = "dbo";

        // Table Names
        public const string Item = "Item";
        public const string Purchase = "Purchase";
        public const string PurchasedItem = "PurchasedItem";
        public const string Category = "Category";
        public const string PriceOverride = "PriceOverride";

        // Column Names
        public const string Id = "Id";
        public const string Name = "Name";
        public const string Description = "Description";
        public const string Price = "Price";
        public const string CategoryId = "CategoryId";
        public const string Stock = "Stock";
        public const string UserId = "UserId";
        public const string Date = "Date";
        public const string IsReturn = "IsReturn";
        public const string PurchaseId = "PurchaseId";
        public const string ItemId = "ItemId";
        public const string Quantity = "Quantity";
        public const string UnitPrice = "UnitPrice";
        public const string PromotionalPrice = "PromotionalPrice";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";
        public const string Email = "Email";
    }
}
