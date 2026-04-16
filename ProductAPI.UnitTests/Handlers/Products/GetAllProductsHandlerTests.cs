using Moq;
using ProductsAPI.Application.Products.Queries.GetAllProducts;
using ProductsAPI.Domain.Common;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Handlers.Products;

public class GetAllProductsHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepo = new();

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WithMappedDtos()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "P1", Description = "D1", Price = 10m, Stock = 5, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "P2", Description = "D2", Price = 20m, Stock = 10, CreatedAt = DateTime.UtcNow }
        };
        var pagedList = new PagedList<Product>(products, totalCount: 2, pageNumber: 1, pageSize: 10);

        _mockRepo.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync(pagedList);

        var handler = new GetAllProductsHandler(_mockRepo.Object);
        var result = await handler.Handle(new GetAllProductsQuery(1, 10));

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.Items.Count());
        Assert.Contains(result.Items, p => p.Name == "P1" && p.Price == 10m);
        Assert.Contains(result.Items, p => p.Name == "P2" && p.Price == 20m);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResult_WhenNoProducts()
    {
        var pagedList = new PagedList<Product>([], totalCount: 0, pageNumber: 1, pageSize: 10);

        _mockRepo.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync(pagedList);

        var handler = new GetAllProductsHandler(_mockRepo.Object);
        var result = await handler.Handle(new GetAllProductsQuery(1, 10));

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationParameters_ToRepository()
    {
        var pagedList = new PagedList<Product>([], totalCount: 0, pageNumber: 2, pageSize: 5);

        _mockRepo.Setup(r => r.GetPagedAsync(2, 5)).ReturnsAsync(pagedList);

        var handler = new GetAllProductsHandler(_mockRepo.Object);
        await handler.Handle(new GetAllProductsQuery(2, 5));

        _mockRepo.Verify(r => r.GetPagedAsync(2, 5), Times.Once);
    }
}
