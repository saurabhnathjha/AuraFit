using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuraFitWebService.DTOs;
using AuraFitWebService.Utilities;
using System.Security.Claims;
using AuraFitDataAccessLayer.Models;
using AuraFitDataAccessLayer.Repositories;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using Serilog;

namespace AuraFitWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGoalsController : ControllerBase
    {
        private readonly UserGoalRepository _goalRepo;
        private readonly IUserProfileRepository _profileRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMealRepository _mealRepo;
        private readonly IWorkoutLogRepository _workoutRepo;
        private readonly Serilog.ILogger _logger;

        public UserGoalsController(
            UserGoalRepository goalRepo,
            IUserProfileRepository profileRepo,
            IHttpContextAccessor httpContextAccessor,
            IMealRepository mealRepo,
            IWorkoutLogRepository workoutRepo)
        {
            _goalRepo = goalRepo;
            _profileRepo = profileRepo;
            _httpContextAccessor = httpContextAccessor;
            _mealRepo = mealRepo;
            _workoutRepo = workoutRepo;
            _logger = Log.Logger;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyGoal()
        {
            try
            {
                int userId = GetUserIdFromClaims();

                var goal = await _goalRepo.GetByUserIdAsync(userId);
                if (goal == null)
                {
                    _logger.Warning("No goals found");
                    return NotFound("User goal not found.");
                }
                else
                {
                    _logger.Information("Goals fetched");
                    return Ok(goal);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured: " + ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrUpdateGoal(UserGoalDTO dto)
        {
            try
            {
                int userId = GetUserIdFromClaims();

                var profile = await _profileRepo.GetByUserIdAsync(userId);
                if (profile == null || profile.WeightKg == null || profile.Tdee == null)
                {
                    _logger.Warning("Goal set fail due to less data");
                    return BadRequest("Incomplete profile data for goal setting.");
                }

                // Determine goal type, weekly target kg and calories
                string goalType = (dto.AutoSuggested || string.IsNullOrEmpty(dto.GoalType))
                    ? GoalCalculator.SuggestGoalType(profile.Bmi ?? throw new InvalidOperationException("BMI is null"))
                    : dto.GoalType;

                double weight = profile.WeightKg.Value;

                double weeklyTargetKg = (dto.AutoSuggested || dto.WeeklyTargetKg == null)
                    ? GoalCalculator.SuggestWeeklyTargetKg(goalType, weight)
                    : dto.WeeklyTargetKg.Value;

                int targetCalories = (dto.AutoSuggested || dto.TargetCalories == null)
                    ? GoalCalculator.CalculateTargetCalories(profile.Tdee.Value, goalType, weight)
                    : dto.TargetCalories.Value;

                var goal = new UserGoal
                {
                    UserId = userId,
                    GoalType = goalType,
                    WeeklyTargetKg = weeklyTargetKg,
                    TargetCalories = targetCalories,
                    AutoSuggested = dto.AutoSuggested,
                    GoalStartDate = dto.GoalStartDate
                };

                bool success = await _goalRepo.UpsertAsync(goal);
                if (!success)
                {
                    _logger.Warning("Failed saving user goal");
                    return StatusCode(500, "Failed to save user goal.");

                }
                else
                {
                    _logger.Information("Goal set success");
                    return Ok(goal);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured: " + ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpGet("progress")]
        [Authorize]
        public async Task<IActionResult> GetMyGoalProgress()
        {
            try
            {
                int userId = GetUserIdFromClaims();
                var today = DateTime.Now.Date;

                var goal = await _goalRepo.GetByUserIdAsync(userId);
                if (goal == null)
                {
                    _logger.Information("No goal available to display");
                    return NotFound("User goal not found.");
                }

                var caloriesConsumed = await _mealRepo.GetTotalCaloriesForUserOnDateAsync(userId, today);
                var caloriesBurned = await _workoutRepo.GetTotalCaloriesBurnedForUserOnDateAsync(userId, today);

                var progressDto = new
                {
                    GoalType = goal.GoalType,
                    WeeklyTargetKg = goal.WeeklyTargetKg,
                    TargetCalories = goal.TargetCalories,
                    CaloriesConsumed = caloriesConsumed,
                    CaloriesBurned = caloriesBurned
                    // Add other progress info if needed
                };
                _logger.Information("Goal progress fetch success.");
                return Ok(progressDto);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured: " + ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("suggested")]
        [Authorize]
        public async Task<IActionResult> GetSuggestedGoal()
        {
            try
            {
                int userId = GetUserIdFromClaims();

                var profile = await _profileRepo.GetByUserIdAsync(userId);
                if (profile == null || profile.WeightKg == null || profile.Tdee == null || profile.Bmi == null)
                {
                    _logger.Warning("Fail to get suggestions less info provided.");
                    return BadRequest("Incomplete profile data for suggestion.");
                }

                string goalType = GoalCalculator.SuggestGoalType(profile.Bmi.Value);
                double weeklyTargetKg = GoalCalculator.SuggestWeeklyTargetKg(goalType, profile.WeightKg.Value);
                int targetCalories = GoalCalculator.CalculateTargetCalories(profile.Tdee.Value, goalType, profile.WeightKg.Value);

                var suggestedGoal = new
                {
                    goalType,
                    weeklyTargetKg,
                    targetCalories
                };
                _logger.Information("Suggestion fetch success.");
                return Ok(suggestedGoal);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured: " + ex.Message);
                return BadRequest(ex);
            }
        }


        private int GetUserIdFromClaims()
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    throw new Exception("User id claim missing");

                return int.Parse(userIdClaim.Value);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured: " + ex.Message);
                return -99;
            }
        }
    }
}
