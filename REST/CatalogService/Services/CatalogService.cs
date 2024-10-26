using CatalogAPI.Models;
using CatalogAPI.Repository.Interfaces;
using CatalogAPI.Services.Interfaces;

namespace CatalogAPI.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IItemRepository _itemRepository;

        public CatalogService(ICategoryRepository categoryRepository, IItemRepository itemRepository)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetCategoryByIdAsync(id);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await _categoryRepository.CreateCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _categoryRepository.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(int categoryId, int pageNumber, int pageSize)
        {
            return await _itemRepository.GetItemsAsync(categoryId, pageNumber, pageSize);
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await _itemRepository.GetItemByIdAsync(id);
        }

        public async Task CreateItemAsync(Item item)
        {
            await _itemRepository.CreateItemAsync(item);
        }

        public async Task UpdateItemAsync(Item item)
        {
            await _itemRepository.UpdateItemAsync(item);
        }

        public async Task DeleteItemAsync(int id)
        {
            await _itemRepository.DeleteItemAsync(id);
        }
    }
}
