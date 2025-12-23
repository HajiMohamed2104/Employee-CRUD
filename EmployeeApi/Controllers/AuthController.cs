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
            _logger.LogInformation("Register process started for user: {Username}", request.Username);

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
                    _logger.LogWarning("Registration failed: Invalid role '{Role}' provided.", request.Role);
                    return BadRequest(new { Message = "Role must be either 'Admin' or 'User'." });
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
                    _logger.LogWarning("Registration failed: User '{Username}' already exists.", request.Username);
                    return Conflict(new { Message = "User already exists." });
                }

                _logger.LogInformation("User '{Username}' registered successfully.", request.Username);
                return Ok(new { Message = "Registration successful.", User = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration for {Username}.", request.Username);
                return StatusCode(500, new { Message = "Internal Server Error. Please try again later." });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            _logger.LogInformation("Login process started for user: {Username}", request.Username);

            try
            {
                var loginResult = await _authService.LoginAsync(request.Username, request.Password);
                if (loginResult == null)
                {
                    _logger.LogWarning("Login failed: Invalid username or password for '{Username}'.", request.Username);
                    return Unauthorized(new { Message = "Invalid username or password." });
                }

                _logger.LogInformation("Login successful for user: {Username}. Role: {Role}. Token generated.", request.Username, loginResult.Role);
                return Ok(new { Message = $"{loginResult.Role} logged in successfully.", Token = loginResult.Token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {Username}.", request.Username);
                return StatusCode(500, new { Message = "Internal Server Error. Please try again later." });
            }
        }
    }
}