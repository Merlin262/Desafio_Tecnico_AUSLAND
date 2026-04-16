using Moq;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Services;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_mockRepo.Object);
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProductDtos()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "P1", Description = "D1", Price = 10m, Stock = 5, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "P2", Description = "D2", Price = 20m, Stock = 10, CreatedAt = DateTime.UtcNow }
        };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "P1" && p.Price == 10m);
        Assert.Contains(result, p => p.Name == "P2" && p.Price == 20m);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoProducts()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductDto_WhenFound()
    {
        var createdAt = DateTime.UtcNow;
        var product = new Product { Id = 1, Name = "Widget", Description = "A widget", Price = 9.99m, Stock = 100, CreatedAt = createdAt };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Widget", result.Name);
        Assert.Equal("A widget", result.Description);
        Assert.Equal(9.99m, result.Price);
        Assert.Equal(100, result.Stock);
        Assert.Equal(createdAt, result.CreatedAt);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_AndReturnDto()
    {
        var dto = new CreateProductDto("Widget", "A widget", 9.99m, 100);
        var created = new Product { Id = 1, Name = "Widget", Description = "A widget", Price = 9.99m, Stock = 100, CreatedAt = DateTime.UtcNow };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(created);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Widget", result.Name);
        _mockRepo.Verify(r => r.CreateAsync(It.Is<Product>(p =>
            p.Name == "Widget" &&
            p.Price == 9.99m &&
            p.Stock == 100
        )), Times.Once);
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenProductNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var result = await _service.UpdateAsync(99, new UpdateProductDto("Name", "Desc", 1m, 1));

        Assert.False(result);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_AndReturnTrue_WhenFound()
    {
        var original = new Product { Id = 1, Name = "Old", Description = "Old", Price = 5m, Stock = 5, CreatedAt = DateTime.UtcNow.AddDays(-1) };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(original);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(1, new UpdateProductDto("New", "New desc", 15m, 30));

        Assert.True(result);
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
            p.Id == 1 &&
            p.Name == "New" &&
            p.Description == "New desc" &&
            p.Price == 15m &&
            p.Stock == 30 &&
            p.CreatedAt == original.CreatedAt
        )), Times.Once);
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotFound()
    {
        _mockRepo.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenProductDeleted()
    {
        _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
