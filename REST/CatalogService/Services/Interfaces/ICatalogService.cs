using CatalogAPI.Models;

namespace CatalogAPI.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);

        Task<IEnumerable<Item>> GetItemsAsync(int categoryId, int pageNumber, int pageSize);
        Task<Item> GetItemByIdAsync(int id);
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(int id);
    }
}
