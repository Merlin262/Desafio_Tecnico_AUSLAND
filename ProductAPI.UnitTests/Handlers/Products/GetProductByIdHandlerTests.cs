using Moq;
using ProductsAPI.Application.Products.Queries.GetProductById;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Handlers.Products;

public class GetProductByIdHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var handler = new GetProductByIdHandler(_mockRepo.Object);
        var result = await handler.Handle(new GetProductByIdQuery(99));

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductDto_WhenProductFound()
    {
        var createdAt = DateTime.UtcNow;
        var product = new Product
        {
            Id = 1,
            Name = "Widget",
            Description = "A widget",
            Price = 9.99m,
            Stock = 100,
            CreatedAt = createdAt
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var handler = new GetProductByIdHandler(_mockRepo.Object);
        var result = await handler.Handle(new GetProductByIdQuery(1));

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Widget", result.Name);
        Assert.Equal("A widget", result.Description);
        Assert.Equal(9.99m, result.Price);
        Assert.Equal(100, result.Stock);
        Assert.Equal(createdAt, result.CreatedAt);
    }
}
