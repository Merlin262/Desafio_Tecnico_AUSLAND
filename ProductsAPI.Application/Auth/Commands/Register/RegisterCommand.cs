using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsAPI.Application.Auth.Commands.Register
{
    public record RegisterCommand(
        string Username,
        string Password,
        string ConfirmPassword);
}
