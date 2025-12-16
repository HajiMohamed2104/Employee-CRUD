using EmployeeApi.DTOs;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (request.Role != "Admin" && request.Role != "User")
            {
                return BadRequest("Role must be either 'Admin' or 'User'.");
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = string.Empty, // Will be hashed in service
                Role = request.Role
            };

            var result = await _authService.RegisterAsync(user, request.Password);
            if (result == null)
            {
                return BadRequest("User already exists.");
            }

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var token = await _authService.LoginAsync(request.Username, request.Password);
            if (token == null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(new { Token = token });
        }
    }
}
