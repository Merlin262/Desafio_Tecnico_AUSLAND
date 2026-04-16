using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProductsAPI.Application.Auth.Commands.Login
{
    public class LoginHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _jwt;
        private readonly InMemoryUserStore _inMemoryUserStore;

        public LoginHandler(IUserRepository userRepository, JwtTokenService jwt, InMemoryUserStore inMemoryUserStore)
        {
            _userRepository = userRepository;
            _jwt = jwt;
            _inMemoryUserStore = inMemoryUserStore;
        }

        public async Task<AuthResponseDto?> Handle(LoginCommand command)
        {
            // Check hardcoded in-memory user first
            if (command.Username == _inMemoryUserStore.Username &&
                PasswordHasher.Verify(command.Password, _inMemoryUserStore.PasswordHash))
                return new AuthResponseDto(_jwt.Generate(_inMemoryUserStore.Username), _inMemoryUserStore.Username);

            // Fall back to database users
            var user = await _userRepository.GetByUsernameAsync(command.Username);
            if (user is null || !PasswordHasher.Verify(command.Password, user.PasswordHash))
                return null;

            return new AuthResponseDto(_jwt.Generate(user.Username), user.Username);
        }
    }
}
