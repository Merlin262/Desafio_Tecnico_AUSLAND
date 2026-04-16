using ProductsAPI.Application.DTOs;

namespace ProductsAPI.Application.Interfaces
{
    public interface IAuthService
    {
        string? Authenticate(LoginDto loginDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    }
}
