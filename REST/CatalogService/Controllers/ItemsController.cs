using CatalogAPI.Models;
using CatalogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public ItemsController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        /// <summary>
        /// Retrieves items by category.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems([FromQuery] int categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var items = await _catalogService.GetItemsAsync(categoryId, pageNumber, pageSize);
            if (items == null || !items.Any())
            {
                return NotFound("No items found for the specified category.");
            }
            return Ok(items);
        }

        /// <summary>
        /// Retrieves an item by ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            var item = await _catalogService.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        /// <summary>
        /// Adds a new item.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Item>> AddItem([FromBody] Item item)
        {
            await _catalogService.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }

        /// <summary>
        /// Updates an existing item.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Item item)
        {
            if (id != item.Id)
            {
                return BadRequest("Item ID mismatch.");
            }

            var existingItem = await _catalogService.GetItemByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            await _catalogService.UpdateItemAsync(item);
            return NoContent();
        }

        /// <summary>
        /// Deletes an item by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _catalogService.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _catalogService.DeleteItemAsync(id);
            return NoContent();
        }
    }
}
