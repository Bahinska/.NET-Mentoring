using CatalogAPI.Models;
using CatalogAPI.Repository.DB;
using CatalogAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly CatalogContext _context;

        public ItemRepository(CatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(int categoryId, int pageNumber, int pageSize)
        {
            return await _context.Items
                .Where(i => i.CategoryId == categoryId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await _context.Items.FindAsync(id);
        }

        public async Task CreateItemAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
