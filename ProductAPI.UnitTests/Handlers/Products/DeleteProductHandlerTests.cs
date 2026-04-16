using Moq;
using ProductsAPI.Application.Products.Commands.DeleteProduct;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Handlers.Products;

public class DeleteProductHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProductNotFound()
    {
        _mockRepo.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        var handler = new DeleteProductHandler(_mockRepo.Object);
        var result = await handler.Handle(new DeleteProductCommand(99));

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenProductDeleted()
    {
        _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var handler = new DeleteProductHandler(_mockRepo.Object);
        var result = await handler.Handle(new DeleteProductCommand(1));

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldCallRepository_WithCorrectId()
    {
        _mockRepo.Setup(r => r.DeleteAsync(5)).ReturnsAsync(true);

        var handler = new DeleteProductHandler(_mockRepo.Object);
        await handler.Handle(new DeleteProductCommand(5));

        _mockRepo.Verify(r => r.DeleteAsync(5), Times.Once);
    }
}
