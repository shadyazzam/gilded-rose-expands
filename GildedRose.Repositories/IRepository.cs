using GildedRose.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Repositories
{
    /// <summary>
    /// Repository Pattern Interface.
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    public interface IRepository<T> : IDisposable where T : IEntity
    {
        /// <summary>
        /// Get an entity given its Id
        /// </summary>
        /// <param name="id">The unique identifier (primary key)</param>
        /// <returns>The entity identified by Id</returns>
        T Get(Guid id);

        /// <summary>
        /// Gets entity(-ies) using the specified filter and order.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order.</param>
        /// <returns>List of Entities that satisfy the filter, ordered as specified</returns>
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        /// <summary>
        /// Saves the pending changes to the database.
        /// </summary>
        void Save();
        
        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Create(T entity);

        /// <summary>
        /// Delete an entity from the database
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns>Number of objects deleted</returns>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// Deletes the entities that match the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        void Delete(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Counts the entities that match the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The number of entites that match the filter.</returns>
        int Count(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Indicates if any entities exist that match the specified filter
        /// </summary>
        /// <param name="filter">The entity filter.</param>
        /// <returns>true if there are matches, false otherwise.</returns>
        bool Exists(Expression<Func<T, bool>> filter);
    }
}
