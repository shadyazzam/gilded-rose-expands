using GildedRose.DataContracts;
using GildedRose.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace GildedRose.Controllers
{
    [Authorize]
    public class ItemController : ApiController
    {
        private readonly IItemService _itemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemController" /> class.
        /// </summary>
        /// <param name="itemService">The items business logic service.</param>
        /// <param name="categoryService">The category business logic service.</param>
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns>The items.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/items")]
        public List<ItemDto> GetItems()
        {
            return _itemService.GetItems()
                .Select(i => i.ToDto())
                .ToList();
        }

        /// <summary>
        /// Gets the specified item.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>The specified item.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/items/{itemId}", Name = "ItemByItemId")]
        public IHttpActionResult GetItemById(Guid itemId)
        {
            var item = _itemService.GetItemById(itemId);
            if (item == null) return NotFound();
            
            return Ok(item.ToDto());
        }

        /// <summary>
        /// Gets items by category.
        /// </summary>
        /// <param name="cagtegoryId">The category identifier.</param>
        /// <returns>The items matching the specified category.</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/categories/{categoryId}/items", Name = "ItemsByCategoryId")]
        public List<ItemDto> GetItemsByCategoryId(Guid categoryId)
        {
            return _itemService.GetItemsByCategory(categoryId)
                .Select(i => i.ToDto())
                .ToList();
        }
    }
}
