using Microsoft.EntityFrameworkCore;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Data;
using ProductsAPI.Infrastructure.Repositories;

namespace ProductAPI.UnitTests.Repositories;

public class ProductRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);


    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenFound()
    {
        await using var context = CreateContext();
        var product = new Product { Name = "Widget", Description = "A widget", Price = 9.99m, Stock = 10 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var result = await new ProductRepository(context).GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Widget", result.Name);
        Assert.Equal(9.99m, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var result = await new ProductRepository(context).GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductIsSoftDeleted()
    {
        await using var context = CreateContext();
        var product = new Product { Name = "Deleted", Description = "D", Price = 1m, Stock = 1, IsDeleted = true };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var result = await new ProductRepository(context).GetByIdAsync(product.Id);

        Assert.Null(result);
    }

    // --- GetPagedAsync ---

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage_OrderedByCreatedAt()
    {
        await using var context = CreateContext();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        for (int i = 1; i <= 12; i++)
        {
            context.Products.Add(new Product
            {
                Name = $"Product {i:D2}",
                Description = "D",
                Price = i * 1m,
                Stock = i,
                CreatedAt = baseDate.AddSeconds(i)
            });
        }
        await context.SaveChangesAsync();

        var result = await new ProductRepository(context).GetPagedAsync(pageNumber: 2, pageSize: 5);

        Assert.Equal(12, result.TotalCount);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal("Product 06", result.Items[0].Name);
        Assert.Equal("Product 10", result.Items[4].Name);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldNotIncludeDeletedProducts()
    {
        await using var context = CreateContext();
        context.Products.AddRange(
            new Product { Name = "Active",  Description = "D", Price = 1m, Stock = 1 },
            new Product { Name = "Deleted", Description = "D", Price = 1m, Stock = 1, IsDeleted = true }
        );
        await context.SaveChangesAsync();

        var result = await new ProductRepository(context).GetPagedAsync(1, 10);

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Active", result.Items[0].Name);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnLastPage_WhenItemsAreLessThanPageSize()
    {
        await using var context = CreateContext();
        for (int i = 1; i <= 3; i++)
            context.Products.Add(new Product { Name = $"P{i}", Description = "D", Price = i, Stock = i });
        await context.SaveChangesAsync();

        var result = await new ProductRepository(context).GetPagedAsync(pageNumber: 1, pageSize: 10);

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_ShouldPersistProduct_AndReturnWithGeneratedId()
    {
        await using var context = CreateContext();
        var product = new Product { Name = "New Product", Description = "Desc", Price = 19.99m, Stock = 50 };

        var result = await new ProductRepository(context).CreateAsync(product);

        Assert.True(result.Id > 0);
        Assert.Equal("New Product", result.Name);

        context.ChangeTracker.Clear();
        var saved = await context.Products.FindAsync(result.Id);
        Assert.NotNull(saved);
        Assert.Equal("New Product", saved!.Name);
        Assert.Equal(19.99m, saved.Price);
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges_InDatabase()
    {
        await using var context = CreateContext();
        var original = new Product { Name = "Original", Description = "Old desc", Price = 5m, Stock = 5 };
        context.Products.Add(original);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var updated = new Product
        {
            Id = original.Id,
            Name = "Updated",
            Description = "New desc",
            Price = 25m,
            Stock = 100,
            CreatedAt = original.CreatedAt
        };

        await new ProductRepository(context).UpdateAsync(updated);

        context.ChangeTracker.Clear();
        var saved = await context.Products.FindAsync(original.Id);
        Assert.NotNull(saved);
        Assert.Equal("Updated", saved!.Name);
        Assert.Equal("New desc", saved.Description);
        Assert.Equal(25m, saved.Price);
        Assert.Equal(100, saved.Stock);
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotFound()
    {
        await using var context = CreateContext();
        var result = await new ProductRepository(context).DeleteAsync(999);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetIsDeleted_AndReturnTrue()
    {
        await using var context = CreateContext();
        var product = new Product { Name = "ToDelete", Description = "D", Price = 1m, Stock = 1 };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await new ProductRepository(context).DeleteAsync(product.Id);

        Assert.True(result);

        // IgnoreQueryFilters para inspecionar o produto deletado no banco
        context.ChangeTracker.Clear();
        var deleted = await context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == product.Id);

        Assert.NotNull(deleted);
        Assert.True(deleted!.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldMakeProduct_InvisibleToSubsequentQueries()
    {
        await using var context = CreateContext();
        var product = new Product { Name = "ToDelete", Description = "D", Price = 1m, Stock = 1 };
        context.Products.Add(product);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repo = new ProductRepository(context);
        await repo.DeleteAsync(product.Id);

        var found = await repo.GetByIdAsync(product.Id);
        Assert.Null(found);
    }
}
