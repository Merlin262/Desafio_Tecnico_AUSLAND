using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Application.Auth.Commands.Login;
using ProductsAPI.Application.Auth.Commands.Register;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Application.Interfaces;
using Wolverine;

namespace ProductsAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMessageBus _bus;

        public AuthController(IMessageBus bus) => _bus = bus;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _bus.InvokeAsync<AuthResponseDto?>(command);
            if (result is null)
                return Unauthorized(new { message = "Usuário ou senha inválidos." });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            if (command.Password != command.ConfirmPassword)
                return BadRequest(new { message = "As senhas não coincidem." });

            var result = await _bus.InvokeAsync<AuthResponseDto?>(command);
            if (result is null)
                return Conflict(new { message = "Nome de usuário já existe." });

            return Created(string.Empty, result);
        }
    }
}
