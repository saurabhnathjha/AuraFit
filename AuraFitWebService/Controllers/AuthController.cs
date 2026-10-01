using Microsoft.AspNetCore.Mvc;
using AuraFitDataAccessLayer.Models;
using AuraFitWebService.DTOs;
using AuraFitWebService.Utilities;
using AuraFitDataAccessLayer.Repositories.Interfaces;

namespace AuraFitWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository userRepository, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByUsernameAsync(dto.Username);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: Username '{Username}' already exists", dto.Username);
                    return BadRequest("Username already exists");
                }

                var hashedPassword = PasswordHasher.Hash(dto.Password);

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = hashedPassword
                };

                await _userRepository.AddUserAsync(user);
                _logger.LogInformation("User '{Username}' registered successfully", user.Username);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user '{Username}'", dto.Username);
                return BadRequest("An error occurred during registration.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO dto)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(dto.Username);
                if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login attempt failed: Invalid credentials for username '{Username}'", dto.Username);
                    return Unauthorized("Invalid credentials");
                }

                var token = _tokenService.CreateToken(user);
                _logger.LogInformation("User '{Username}' logged in successfully", user.Username);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user '{Username}'", dto.Username);
                return BadRequest("An error occurred during login.");
            }
        }
    }
}
