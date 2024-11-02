using CatalogAPI.Controllers;
using CatalogAPI.Models;
using CatalogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CatalogAPI.Tests
{
    public class CatalogControllerTests
    {
        private readonly Mock<ICatalogService> _mockCatalogService;
        private readonly CatalogController _controller;

        public CatalogControllerTests()
        {
            _mockCatalogService = new Mock<ICatalogService>();
            _controller = new CatalogController(_mockCatalogService.Object);
        }
        [Fact]
        public async Task GetCategories_ReturnsOkObjectResult_WithCategories()
        {
            var fakeCategories = new List<Category> { new Category { Id = 1, Name = "Electronics" } };
            _mockCatalogService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(fakeCategories);

            var result = await _controller.GetCategories();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategories = Assert.IsType<List<Category>>(okResult.Value);
            Assert.Single(returnedCategories);
            Assert.Equal("Electronics", returnedCategories.First().Name);
        }
        [Fact]
        public async Task AddCategory_ReturnsCreatedAtActionResult_WithCategory()
        {
            var newCategory = new Category { Name = "Books" };
            _mockCatalogService.Setup(s => s.CreateCategoryAsync(It.IsAny<Category>()))
                               .Callback<Category>((category) => category.Id = 1);

            var result = await _controller.AddCategory(newCategory);

            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetCategoryById", createdAtResult.ActionName);
            Assert.Equal(1, ((Category)createdAtResult.Value).Id);
        }

        [Fact]
        public async Task GetCategoryById_CategoryExists_ReturnsOkObjectResult()
        {
            var fakeCategory = new Category { Id = 1, Name = "Electronics" };
            _mockCatalogService.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync(fakeCategory);

            var result = await _controller.GetCategoryById(1);

            Assert.Equal("Electronics", result.Value.Name);
        }

        [Fact]
        public async Task GetCategoryById_CategoryDoesNotExist_ReturnsNotFoundResult()
        {
            _mockCatalogService.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync((Category)null);

            var result = await _controller.GetCategoryById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCategory_CategoryExists_ReturnsNoContentResult()
        {
            var existingCategory = new Category { Id = 1, Name = "Books" };
            _mockCatalogService.Setup(s => s.UpdateCategoryAsync(existingCategory)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateCategory(1, existingCategory);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateCategory_CategoryIdMismatch_ReturnsBadRequestResult()
        {
            var existingCategory = new Category { Id = 2, Name = "Books" };

            var result = await _controller.UpdateCategory(1, existingCategory);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteCategory_CategoryExists_ReturnsNoContentResult()
        {
            _mockCatalogService.Setup(s => s.DeleteCategoryAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteCategory(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetItems_CategoryHasItems_ReturnsOkObjectResult()
        {
            var fakeItems = new List<Item> { new Item { Id = 1, Name = "Laptop", CategoryId = 1 } };
            _mockCatalogService.Setup(s => s.GetItemsAsync(1, 1, 10)).ReturnsAsync(fakeItems);

            var result = await _controller.GetItems(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsType<List<Item>>(okResult.Value);
            Assert.Single(items);
            Assert.Equal("Laptop", items[0].Name);
        }

        [Fact]
        public async Task GetItems_CategoryHasNoItems_ReturnsNotFoundResult()
        {
            _mockCatalogService.Setup(s => s.GetItemsAsync(1, 1, 10)).ReturnsAsync(new List<Item>());

            var result = await _controller.GetItems(1);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No items found for the specified category.", notFoundResult.Value);
        }
    }
}
