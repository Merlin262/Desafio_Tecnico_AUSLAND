using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsAPI.Application.Auth.Commands.Login
{
    public record LoginCommand(string Username, string Password);
}
