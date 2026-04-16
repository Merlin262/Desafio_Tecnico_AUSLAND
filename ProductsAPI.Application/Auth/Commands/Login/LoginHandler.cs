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

        public LoginHandler(IUserRepository userRepository, JwtTokenService jwt)
        {
            _userRepository = userRepository;
            _jwt = jwt;
        }

        public async Task<AuthResponseDto?> Handle(LoginCommand command)
        {
            var user = await _userRepository.GetByUsernameAsync(command.Username);
            if (user is null || !PasswordHasher.Verify(command.Password, user.PasswordHash))
                return null;

            return new AuthResponseDto(_jwt.Generate(user.Username), user.Username);
        }
    }
}
