using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsAPI.Application.DTOs
{
    public record RegisterDto(string Username, string Password, string ConfirmPassword);

    public record AuthResponseDto(string Token, string Username);
}
