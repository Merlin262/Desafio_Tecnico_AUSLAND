using Microsoft.Extensions.Configuration;
using Moq;
using ProductsAPI.Application.Auth;
using ProductsAPI.Application.Auth.Commands.Register;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;

namespace ProductAPI.UnitTests.Handlers.Auth;

public class RegisterHandlerTests
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
    public async Task Handle_ShouldReturnNull_WhenUserAlreadyExists()
    {
        _mockRepo.Setup(r => r.ExistsAsync("existinguser")).ReturnsAsync(true);

        var handler = new RegisterHandler(_mockRepo.Object, CreateJwtService());
        var result = await handler.Handle(new RegisterCommand("existinguser", "pass123", "pass123"));

        Assert.Null(result);
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_AndReturnAuthResponse_WhenUserIsNew()
    {
        _mockRepo.Setup(r => r.ExistsAsync("newuser")).ReturnsAsync(false);
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var handler = new RegisterHandler(_mockRepo.Object, CreateJwtService());
        var result = await handler.Handle(new RegisterCommand("newuser", "password123", "password123"));

        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.NotEmpty(result.Token);
        _mockRepo.Verify(r => r.CreateAsync(It.Is<User>(u =>
            u.Username == "newuser" &&
            !string.IsNullOrEmpty(u.PasswordHash)
        )), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldHashPassword_BeforeSavingUser()
    {
        User? savedUser = null;
        _mockRepo.Setup(r => r.ExistsAsync("newuser")).ReturnsAsync(false);
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => savedUser = u)
            .ReturnsAsync((User u) => u);

        var handler = new RegisterHandler(_mockRepo.Object, CreateJwtService());
        await handler.Handle(new RegisterCommand("newuser", "plaintext", "plaintext"));

        Assert.NotNull(savedUser);
        Assert.NotEqual("plaintext", savedUser!.PasswordHash);
        Assert.Contains(".", savedUser.PasswordHash); // format: salt.hash
    }
}
