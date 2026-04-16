using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductsAPI.Application.Auth;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductsAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public string? Authenticate(LoginDto loginDto)
        {
            var userTask = _userRepository.GetByUsernameAsync(loginDto.Username);
            userTask.Wait();
            var user = userTask.Result;
            if (user is null || !PasswordHasher.Verify(loginDto.Password, user.PasswordHash))
                return null;

            return GenerateToken(user.Username);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user is null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                return null;

            return new AuthResponseDto(GenerateToken(user.Username), user.Username);
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var exists = await _userRepository.ExistsAsync(dto.Username);
            if (exists) return null;

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = PasswordHasher.Hash(dto.Password)
            };

            await _userRepository.CreateAsync(user);
            return new AuthResponseDto(GenerateToken(user.Username), user.Username);
        }

        private string GenerateToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: [new Claim(ClaimTypes.Name, username)],
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
