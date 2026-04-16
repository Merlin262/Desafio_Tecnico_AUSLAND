using Moq;
using ProductsAPI.Application.Products.Commands.UpdateProduct;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Handlers.Products;

public class UpdateProductHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var handler = new UpdateProductHandler(_mockRepo.Object);
        var result = await handler.Handle(new UpdateProductCommand(99, "Name", "Desc", 1m, 1));

        Assert.False(result);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProduct_AndReturnTrue_WhenFound()
    {
        var existing = new Product
        {
            Id = 1,
            Name = "Old Name",
            Description = "Old description",
            Price = 5m,
            Stock = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var handler = new UpdateProductHandler(_mockRepo.Object);
        var result = await handler.Handle(new UpdateProductCommand(1, "New Name", "New description", 10m, 20));

        Assert.True(result);
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
            p.Id == 1 &&
            p.Name == "New Name" &&
            p.Description == "New description" &&
            p.Price == 10m &&
            p.Stock == 20 &&
            p.CreatedAt == existing.CreatedAt
        )), Times.Once);
    }
}
