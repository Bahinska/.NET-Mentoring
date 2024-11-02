using CatalogAPI.Controllers;
using CatalogAPI.Models;
using CatalogAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CatalogAPI.Tests
{
    public class ItemsControllerTests
    {
        private readonly Mock<ICatalogService> _mockCatalogService;
        private readonly ItemsController _controller;

        public ItemsControllerTests()
        {
            _mockCatalogService = new Mock<ICatalogService>();
            _controller = new ItemsController(_mockCatalogService.Object);
        }

        [Fact]
        public async Task GetItems_CategoryExists_ReturnsOkObjectResult()
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
        public async Task GetItems_CategoryDoesNotExist_ReturnsNotFoundResult()
        {
            _mockCatalogService.Setup(s => s.GetItemsAsync(1, 1, 10)).ReturnsAsync(new List<Item>());

            var result = await _controller.GetItems(1);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No items found for the specified category.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetItemById_ItemExists_ReturnsOkObjectResult()
        {
            var fakeItem = new Item { Id = 1, Name = "Laptop", CategoryId = 1 };
            _mockCatalogService.Setup(s => s.GetItemByIdAsync(1)).ReturnsAsync(fakeItem);

            var result = await _controller.GetItemById(1);

            Assert.Equal("Laptop", result.Value.Name);
        }

        [Fact]
        public async Task GetItemById_ItemDoesNotExist_ReturnsNotFoundResult()
        {
            _mockCatalogService.Setup(s => s.GetItemByIdAsync(1)).ReturnsAsync((Item)null);

            var result = await _controller.GetItemById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddItem_ValidItem_ReturnsCreatedAtActionResult()
        {
            var newItem = new Item { Name = "Smartphone", CategoryId = 1 };
            _mockCatalogService.Setup(s => s.CreateItemAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);

            var result = await _controller.AddItem(newItem);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetItemById", createdAtActionResult.ActionName);
            Assert.Equal(newItem, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UpdateItem_ItemExists_ReturnsNoContentResult()
        {
            var existingItem = new Item { Id = 1, Name = "Smartphone", CategoryId = 1 };
            _mockCatalogService.Setup(s => s.UpdateItemAsync(It.IsAny<Item>())).Returns(Task.CompletedTask);

            var result = await _controller.UpdateItem(1, existingItem);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateItem_IdMismatch_ReturnsBadRequestResult()
        {
            var existingItem = new Item { Id = 2, Name = "Smartphone", CategoryId = 1 };

            var result = await _controller.UpdateItem(1, existingItem);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteItem_ItemExists_ReturnsNoContentResult()
        {
            _mockCatalogService.Setup(s => s.DeleteItemAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteItem(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteItem_ItemDoesNotExist_ReturnsNotFoundResult()
        {
            _mockCatalogService.Setup(s => s.GetItemByIdAsync(1)).ReturnsAsync((Item)null);

            var result = await _controller.DeleteItem(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
