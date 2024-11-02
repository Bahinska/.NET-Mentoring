using CatalogAPI.Models;

namespace CatalogAPI.Repository.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetItemsAsync(int categoryId, int pageNumber, int pageSize);
        Task<Item> GetItemByIdAsync(int id);
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(int id);
    }
}
