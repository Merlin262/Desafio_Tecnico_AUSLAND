using Microsoft.Extensions.Configuration;
using Moq;
using ProductsAPI.Application.Auth;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Services;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo = new();
    private readonly Mock<IConfiguration> _mockConfig = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("super-secret-key-for-unit-testing-only!");
        _mockConfig.Setup(c => c["Jwt:ExpiresInMinutes"]).Returns("60");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _service = new AuthService(_mockRepo.Object, _mockConfig.Object);
    }

    // --- LoginAsync ---

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
    {
        _mockRepo.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User?)null);

        var result = await _service.LoginAsync(new LoginDto { Username = "unknown", Password = "pass" });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsWrong()
    {
        var hash = PasswordHasher.Hash("correctpassword");
        var user = new User { Id = 1, Username = "john", PasswordHash = hash };

        _mockRepo.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginDto { Username = "john", Password = "wrongpassword" });

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var hash = PasswordHasher.Hash("secret123");
        var user = new User { Id = 1, Username = "john", PasswordHash = hash };

        _mockRepo.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginDto { Username = "john", Password = "secret123" });

        Assert.NotNull(result);
        Assert.Equal("john", result.Username);
        Assert.NotEmpty(result.Token);
    }

    // --- RegisterAsync ---

    [Fact]
    public async Task RegisterAsync_ShouldReturnNull_WhenUserAlreadyExists()
    {
        _mockRepo.Setup(r => r.ExistsAsync("existinguser")).ReturnsAsync(true);

        var result = await _service.RegisterAsync(new RegisterDto("existinguser", "pass123", "pass123"));

        Assert.Null(result);
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_AndReturnAuthResponse_WhenNew()
    {
        _mockRepo.Setup(r => r.ExistsAsync("newuser")).ReturnsAsync(false);
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var result = await _service.RegisterAsync(new RegisterDto("newuser", "password123", "password123"));

        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.NotEmpty(result.Token);
        _mockRepo.Verify(r => r.CreateAsync(It.Is<User>(u =>
            u.Username == "newuser" &&
            !string.IsNullOrEmpty(u.PasswordHash)
        )), Times.Once);
    }

    // --- Authenticate (sync) ---

    [Fact]
    public void Authenticate_ShouldReturnNull_WhenUserNotFound()
    {
        _mockRepo.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User?)null);

        var result = _service.Authenticate(new LoginDto { Username = "unknown", Password = "pass" });

        Assert.Null(result);
    }

    [Fact]
    public void Authenticate_ShouldReturnNull_WhenPasswordIsWrong()
    {
        var hash = PasswordHasher.Hash("correctpassword");
        var user = new User { Id = 1, Username = "john", PasswordHash = hash };

        _mockRepo.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

        var result = _service.Authenticate(new LoginDto { Username = "john", Password = "wrongpassword" });

        Assert.Null(result);
    }

    [Fact]
    public void Authenticate_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var hash = PasswordHasher.Hash("secret123");
        var user = new User { Id = 1, Username = "john", PasswordHash = hash };

        _mockRepo.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

        var result = _service.Authenticate(new LoginDto { Username = "john", Password = "secret123" });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
