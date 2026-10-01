using AuraFitDataAccessLayer.Models;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using AuraFitWebService.DTOs;
using AuraFitWebService.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AuraFitWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            IUserProfileRepository repo,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserProfileController> logger)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (userId <= 0)
                {
                    _logger.LogWarning("Invalid user ID in claims.");
                    return Unauthorized();
                }

                var profile = await _repo.GetByUserIdAsync(userId);
                if (profile == null)
                {
                    _logger.LogWarning("No profile found for user ID {UserId}", userId);
                    return NotFound();
                }

                _logger.LogInformation("Profile fetched for user ID {UserId}", userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user profile.");
                return StatusCode(500, "Failed to fetch profile.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrUpdateProfile(UserProfileDTO dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (userId <= 0)
                {
                    _logger.LogWarning("Invalid user ID in claims during profile update.");
                    return Unauthorized();
                }

                var bmi = FitnessCalculator.CalculateBMI(dto.WeightKg, dto.HeightCm);
                var bmr = FitnessCalculator.CalculateBMR(dto.WeightKg, dto.HeightCm, dto.Age, dto.Gender);
                var tdee = FitnessCalculator.CalculateTDEE(bmr, dto.ActivityLevel);

                var profile = new UserProfile
                {
                    UserId = userId,
                    Age = dto.Age,
                    Gender = dto.Gender,
                    HeightCm = dto.HeightCm,
                    WeightKg = dto.WeightKg,
                    ActivityLevel = dto.ActivityLevel,
                    Bmi = bmi,
                    Bmr = bmr,
                    Tdee = tdee
                };

                var success = await _repo.UpsertAsync(profile);

                WeightLog weightLog = new WeightLog()
                {
                    UserId = userId,
                    WeightKg = dto.WeightKg,
                    LoggedAt = DateTime.Now
                };

                if (success)
                {
                    await _repo.SendDataToWeightLogs(weightLog);
                    _logger.LogInformation("User profile upserted successfully for user ID {UserId}", userId);
                    return Ok(profile);
                }


                _logger.LogWarning("Failed to upsert profile for user ID {UserId}", userId);
                return StatusCode(500, "Could not save profile.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating/updating profile.");
                return StatusCode(500, "Failed to save profile.");
            }
        }

        [Authorize]
        [HttpGet("shouldUpdateWeight")]
        public async Task<IActionResult> ShouldUpdateWeight()
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var shouldUpdate = await _repo.ShouldPromptWeightUpdateAsync(userId);
                _logger.LogInformation("User needs to update his weight.");
                return Ok(new { shouldUpdate });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching profile info.");
                return BadRequest(ex);
            }
        }


        private int GetUserIdFromClaims()
        {
            try
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    _logger.LogWarning("User ID claim missing in token.");
                    return -1;
                }

                if (!int.TryParse(claim.Value, out var userId))
                {
                    _logger.LogWarning("Failed to parse user ID claim: {ClaimValue}", claim.Value);
                    return -1;
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract user ID from claims.");
                return -1;
            }
        }
    }
}
