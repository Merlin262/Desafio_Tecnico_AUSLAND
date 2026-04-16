using Moq;
using ProductsAPI.Application.Products.Commands.CreateProduct;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Handlers.Products;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();

    [Fact]
    public async Task Handle_ShouldCreateProduct_AndReturnProductDto()
    {
        var command = new CreateProductCommand("Widget", "A widget", 9.99m, 100);
        var createdAt = DateTime.UtcNow;
        var created = new Product
        {
            Id = 1,
            Name = "Widget",
            Description = "A widget",
            Price = 9.99m,
            Stock = 100,
            CreatedAt = createdAt
        };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(created);

        var handler = new CreateProductHandler(_mockRepo.Object);
        var result = await handler.Handle(command);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Widget", result.Name);
        Assert.Equal("A widget", result.Description);
        Assert.Equal(9.99m, result.Price);
        Assert.Equal(100, result.Stock);
        Assert.Equal(createdAt, result.CreatedAt);
    }

    [Fact]
    public async Task Handle_ShouldCallRepository_WithCorrectProductData()
    {
        var command = new CreateProductCommand("Gadget", "A gadget", 49.90m, 25);
        var created = new Product { Id = 2, Name = "Gadget", Description = "A gadget", Price = 49.90m, Stock = 25 };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(created);

        var handler = new CreateProductHandler(_mockRepo.Object);
        await handler.Handle(command);

        _mockRepo.Verify(r => r.CreateAsync(It.Is<Product>(p =>
            p.Name == "Gadget" &&
            p.Description == "A gadget" &&
            p.Price == 49.90m &&
            p.Stock == 25
        )), Times.Once);
    }
}
