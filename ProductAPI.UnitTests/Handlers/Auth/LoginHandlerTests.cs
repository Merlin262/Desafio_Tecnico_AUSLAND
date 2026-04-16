using Microsoft.Extensions.Configuration;
using Moq;
using ProductsAPI.Application.Auth;
using ProductsAPI.Application.Auth.Commands.Login;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Handlers.Auth;

public class LoginHandlerTests
{
    private readonly Mock<IUserRepository> _mockRepo = new();

    private static JwtTokenService CreateJwtService()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("super-secret-key-for-unit-testing-only!");
        mockConfig.Setup(c => c["Jwt:ExpiresInMinutes"]).Returns("60");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        return new JwtTokenService(mockConfig.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserNotFound()
    {
        _mockRepo.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User?)null);

        var handler = new LoginHandler(_mockRepo.Object, CreateJwtService());
        var result = await handler.Handle(new LoginCommand("unknown", "password"));

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenPasswordIsWrong()
    {
        var hash = PasswordHasher.Hash("correctpassword");
        var user = new User { Id = 1, Username = "john", PasswordHash = hash };

        _mockRepo.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

        var handler = new LoginHandler(_mockRepo.Object, CreateJwtService());
        var result = await handler.Handle(new LoginCommand("john", "wrongpassword"));

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var hash = PasswordHasher.Hash("secret123");
        var user = new User { Id = 1, Username = "john", PasswordHash = hash };

        _mockRepo.Setup(r => r.GetByUsernameAsync("john")).ReturnsAsync(user);

        var handler = new LoginHandler(_mockRepo.Object, CreateJwtService());
        var result = await handler.Handle(new LoginCommand("john", "secret123"));

        Assert.NotNull(result);
        Assert.Equal("john", result.Username);
        Assert.NotEmpty(result.Token);
    }
}
