using EmployeeApi.DTOs;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            try
            {
                string normalizedRole;
                if (string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedRole = "Admin";
                }
                else if (string.Equals(request.Role, "User", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedRole = "User";
                }
                else
                {
                    return StatusCode(401, "Role must be either 'Admin' or 'User'.");
                }

                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = string.Empty,
                    Role = normalizedRole
                };

                var result = await _authService.RegisterAsync(user, request.Password);
                if (result == null)
                {
                    return StatusCode(409, "User already exists.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration for {Username}.", request.Username);
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            try
            {
                var token = await _authService.LoginAsync(request.Username, request.Password);
                if (token == null)
                {
                    return StatusCode(404, "Invalid username or password.");
                }

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {Username}.", request.Username);
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
    }
}