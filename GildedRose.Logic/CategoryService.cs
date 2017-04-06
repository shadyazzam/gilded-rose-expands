using System;
using System.Collections.Generic;
using GildedRose.Entities;
using GildedRose.Repositories;

namespace GildedRose.Logic
{
    /// <summary>
    /// Category Service.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService" /> class.
        /// </summary>
        /// <param name="categoryRepository">The category repository.</param>
        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns>A collection of all Category entities</returns>
        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.Get();
        }

        /// <summary>
        /// Gets a category by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The category matching the specified id, or null if it doesn't exist.</returns>
        public Category GetCategoryById(Guid id)
        {
            return _categoryRepository.Get(id);
        }
    }
}
