using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Domain.Entities;
using ProductsAPI.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProductsAPI.Application.Auth.Commands.Register
{
    public class RegisterHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _jwt;

        public RegisterHandler(IUserRepository userRepository, JwtTokenService jwt)
        {
            _userRepository = userRepository;
            _jwt = jwt;
        }

        public async Task<AuthResponseDto?> Handle(RegisterCommand command)
        {
            if (await _userRepository.ExistsAsync(command.Username)) return null;

            var user = new User
            {
                Username = command.Username,
                PasswordHash = PasswordHasher.Hash(command.Password)
            };

            await _userRepository.CreateAsync(user);
            return new AuthResponseDto(_jwt.Generate(user.Username), user.Username);
        }
    }
}
