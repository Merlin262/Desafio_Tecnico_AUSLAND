using Microsoft.EntityFrameworkCore;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Data;
using ProductsAPI.Infrastructure.Repositories;

namespace ProductAPI.UnitTests.Repositories;

public class UserRepositoryTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    // --- GetByUsernameAsync ---

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnUser_WhenFound()
    {
        await using var context = CreateContext();
        context.Users.Add(new User { Username = "john", PasswordHash = "hash123" });
        await context.SaveChangesAsync();

        var result = await new UserRepository(context).GetByUsernameAsync("john");

        Assert.NotNull(result);
        Assert.Equal("john", result.Username);
        Assert.Equal("hash123", result.PasswordHash);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var result = await new UserRepository(context).GetByUsernameAsync("nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnNull_WhenUsernameIsEmpty()
    {
        await using var context = CreateContext();
        var result = await new UserRepository(context).GetByUsernameAsync("");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldNotReturnOtherUsers()
    {
        await using var context = CreateContext();
        context.Users.AddRange(
            new User { Username = "alice", PasswordHash = "hashA" },
            new User { Username = "bob",   PasswordHash = "hashB" }
        );
        await context.SaveChangesAsync();

        var result = await new UserRepository(context).GetByUsernameAsync("alice");

        Assert.NotNull(result);
        Assert.Equal("alice", result.Username);
    }

    // --- ExistsAsync ---

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        await using var context = CreateContext();
        context.Users.Add(new User { Username = "alice", PasswordHash = "hash" });
        await context.SaveChangesAsync();

        var result = await new UserRepository(context).ExistsAsync("alice");

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        await using var context = CreateContext();
        var result = await new UserRepository(context).ExistsAsync("nonexistent");
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenDatabaseIsEmpty()
    {
        await using var context = CreateContext();
        var result = await new UserRepository(context).ExistsAsync("anyone");
        Assert.False(result);
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_ShouldPersistUser_AndReturnWithGeneratedId()
    {
        await using var context = CreateContext();
        var user = new User { Username = "newuser", PasswordHash = "hashedpassword" };

        var result = await new UserRepository(context).CreateAsync(user);

        Assert.True(result.Id > 0);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("hashedpassword", result.PasswordHash);

        context.ChangeTracker.Clear();
        var saved = await context.Users.FindAsync(result.Id);
        Assert.NotNull(saved);
        Assert.Equal("newuser", saved!.Username);
    }

    [Fact]
    public async Task CreateAsync_ShouldMakeUser_AvailableForLookup()
    {
        await using var context = CreateContext();
        var repo = new UserRepository(context);

        await repo.CreateAsync(new User { Username = "carol", PasswordHash = "hash" });

        var found = await repo.GetByUsernameAsync("carol");
        Assert.NotNull(found);
        Assert.Equal("carol", found.Username);
    }

    [Fact]
    public async Task CreateAsync_ShouldMakeUser_DetectableByExistsAsync()
    {
        await using var context = CreateContext();
        var repo = new UserRepository(context);

        await repo.CreateAsync(new User { Username = "dave", PasswordHash = "hash" });

        var exists = await repo.ExistsAsync("dave");
        Assert.True(exists);
    }
}
