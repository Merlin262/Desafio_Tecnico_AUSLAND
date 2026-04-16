using Microsoft.AspNetCore.Mvc;
using ProductsAPI.API.Exceptions;
using ProductsAPI.Application.Auth.Commands.Login;
using ProductsAPI.Application.Auth.Commands.Register;
using ProductsAPI.Application.DTOs;
using Wolverine;

namespace ProductsAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMessageBus bus) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await bus.InvokeAsync<AuthResponseDto?>(command)
                ?? throw new UnauthorizedException();

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            if (command.Password != command.ConfirmPassword)
                throw new BadRequestException("As senhas não coincidem.");

            var result = await bus.InvokeAsync<AuthResponseDto?>(command)
                ?? throw new ConflictException("Nome de usuário já existe.");

            return Created(string.Empty, result);
        }
    }
}
