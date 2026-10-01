using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuraFitDataAccessLayer.Models;
using AuraFitWebService.Services;
using System.Security.Claims;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using Serilog;

namespace AuraFitWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealsController : ControllerBase
    {
        private readonly IMealRepository _mealRepo;
        private readonly NutritionixService _nutritionixService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MealsController> _logger;

        public MealsController(
            IMealRepository mealRepo,
            NutritionixService nutritionixService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MealsController> logger)
        {
            _mealRepo = mealRepo;
            _nutritionixService = nutritionixService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("log")]
        public async Task<IActionResult> LogMeal([FromBody] MealLogRequestDTO dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var nutritionResults = await _nutritionixService.ParseMealQueryAsync(dto.Query);

                if (nutritionResults == null || !nutritionResults.Any())
                {
                    _logger.LogWarning("Failed to parse meal input for user {UserId}", userId);
                    return BadRequest("Could not parse any food items from your input.");
                }

                var totalCalories = nutritionResults.Sum(item => item.Calories);

                var meal = new Meal
                {
                    UserId = userId,
                    Name = dto.Name,
                    Description = dto.Description,
                    LoggedAt = DateTime.UtcNow,
                    Source = "api",
                    TotalCalories = totalCalories,
                    FoodItems = nutritionResults.Select(item => new FoodItem
                    {
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Calories = item.Calories,
                        Source = "api"
                    }).ToList()
                };

                var mealId = await _mealRepo.CreateMealAsync(meal);
                if (mealId <= 0)
                {
                    _logger.LogWarning("Failed to log meal for user {UserId}", userId);
                    return StatusCode(500, "Failed to log meal.");
                }

                meal.MealId = mealId;
                _logger.LogInformation("Meal logged successfully for user {UserId}", userId);
                return Ok(meal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while logging meal");
                return StatusCode(500, "An error occurred while logging the meal.");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllMeals()
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var meals = await _mealRepo.GetMealsByUserIdAsync(userId);
                _logger.LogInformation("Fetched meals for user {UserId}", userId);
                return Ok(meals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while fetching meals");
                return StatusCode(500, "An error occurred while retrieving meals.");
            }
        }

        [Authorize]
        [HttpGet("summary")]
        public async Task<IActionResult> GetMealSummary([FromQuery] DateTime date)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var totalCalories = await _mealRepo.GetTotalCaloriesForUserOnDateAsync(userId, date.Date);
                _logger.LogInformation("Fetched meal summary for user {UserId} on {Date}", userId, date.Date);
                return Ok(new { totalCaloriesConsumed = totalCalories });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while fetching meal summary");
                return StatusCode(500, "An error occurred while retrieving the meal summary.");
            }
        }

        [Authorize]
        [HttpPut("{mealId}")]
        public async Task<IActionResult> UpdateMeal(int mealId, [FromBody] MealLogRequestDTO dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var nutritionResults = await _nutritionixService.ParseMealQueryAsync(dto.Query);

                if (nutritionResults == null || !nutritionResults.Any())
                {
                    _logger.LogWarning("Failed to parse updated meal input for user {UserId}", userId);
                    return BadRequest("Could not parse food items from input.");
                }

                var updatedMeal = new Meal
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    LoggedAt = DateTime.UtcNow,
                    Source = "api",
                    TotalCalories = nutritionResults.Sum(i => i.Calories),
                    FoodItems = nutritionResults.Select(item => new FoodItem
                    {
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Calories = item.Calories,
                        Source = "api"
                    }).ToList()
                };

                var success = await _mealRepo.UpdateMealAsync(userId, mealId, updatedMeal);
                if (success)
                {
                    _logger.LogInformation("Updated meal {MealId} for user {UserId}", mealId, userId);
                    return Ok();
                }

                _logger.LogWarning("Meal {MealId} not found for user {UserId}", mealId, userId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while updating meal {MealId}", mealId);
                return StatusCode(500, "An error occurred while updating the meal.");
            }
        }

        [Authorize]
        [HttpDelete("{mealId}")]
        public async Task<IActionResult> DeleteMeal(int mealId)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var success = await _mealRepo.DeleteMealAsync(userId, mealId);
                if (success)
                {
                    _logger.LogInformation("Deleted meal {MealId} for user {UserId}", mealId, userId);
                    return Ok();
                }

                _logger.LogWarning("Meal {MealId} not found for deletion for user {UserId}", mealId, userId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while deleting meal {MealId}", mealId);
                return StatusCode(500, "An error occurred while deleting the meal.");
            }
        }

        private int GetUserIdFromClaims()
        {
            try
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    _logger.LogWarning("User ID claim missing in token");
                    return -1;
                }

                if (!int.TryParse(claim.Value, out var userId))
                {
                    _logger.LogWarning("Invalid User ID claim format: {ClaimValue}", claim.Value);
                    return -1;
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while extracting user ID from claims");
                return -1;
            }
        }
    }
}
