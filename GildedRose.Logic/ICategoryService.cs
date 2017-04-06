using GildedRose.Entities;
using System;
using System.Collections.Generic;

namespace GildedRose.Logic
{
    /// <summary>
    /// Category Service.
    /// </summary>
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();

        Category GetCategoryById(Guid id);
    }
}
