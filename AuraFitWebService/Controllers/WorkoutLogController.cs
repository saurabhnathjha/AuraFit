using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AuraFitDataAccessLayer.Models;
using AuraFitWebService.DTOs;
using AuraFitWebService.Services;
using AuraFitWebService.Utilities;
using Serilog;
using AuraFitDataAccessLayer.Repositories.Interfaces;

namespace AuraFitWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkoutLogController : ControllerBase
    {
        private readonly AuraFitDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWorkoutLogRepository _workoutRepo;
        private readonly ILogger<WorkoutLogController> _logger;

        public WorkoutLogController(AuraFitDbContext context, IHttpContextAccessor httpContextAccessor, IWorkoutLogRepository workoutRepo, ILogger<WorkoutLogController> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _workoutRepo = workoutRepo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWorkouts()
        {
            try
            {
                var userId = GetUserIdFromClaims();

                var workouts = await _context.WorkoutLogs
                    .Where(w => w.UserId == userId)
                    .Include(w => w.WorkoutLogExercises)
                    .ToListAsync();

                _logger.LogInformation("Fetched {Count} workouts for user {UserId}", workouts.Count, userId);
                return Ok(workouts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch workouts");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWorkout(int id)
        {
            try
            {
                var userId = GetUserIdFromClaims();

                var workout = await _context.WorkoutLogs
                    .Include(w => w.WorkoutLogExercises)
                    .FirstOrDefaultAsync(w => w.WorkoutLogId == id && w.UserId == userId);

                if (workout == null)
                {
                    _logger.LogWarning("Workout {WorkoutId} not found for user {UserId}", id, userId);
                    return NotFound();
                }

                _logger.LogInformation("Fetched workout {WorkoutId} for user {UserId}", id, userId);
                return Ok(workout);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch workout {WorkoutId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWorkout(int id, [FromBody] WorkoutLogCreateDTO dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();

                var workout = await _context.WorkoutLogs
                    .Include(w => w.WorkoutLogExercises)
                    .FirstOrDefaultAsync(w => w.WorkoutLogId == id && w.UserId == userId);

                if (workout == null)
                {
                    _logger.LogWarning("No workout found to update with ID {WorkoutId} for user {UserId}", id, userId);
                    return NotFound();
                }

                workout.WorkoutTemplateId = dto.WorkoutTemplateId;
                workout.DurationMin = dto.DurationMin;
                workout.CaloriesBurned = dto.CaloriesBurned;
                workout.LoggedAt = dto.LoggedAt;

                _context.WorkoutLogExercises.RemoveRange(workout.WorkoutLogExercises);

                workout.WorkoutLogExercises = dto.Exercises.Select(e => new WorkoutLogExercise
                {
                    ExerciseCatalogId = e.ExerciseCatalogId,
                    Reps = e.Reps,
                    Sets = e.Sets,
                    WeightKg = e.WeightKg,
                    DurationMin = e.DurationMin,
                    CaloriesBurned = e.CaloriesBurned,
                    PersonalRecord = e.PersonalRecord,
                    Notes = e.Notes
                }).ToList();

                await _context.SaveChangesAsync();

                _logger.LogInformation("Workout {WorkoutId} updated successfully for user {UserId}", id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update workout {WorkoutId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            try
            {
                var userId = GetUserIdFromClaims();

                var workout = await _context.WorkoutLogs
                    .FirstOrDefaultAsync(w => w.WorkoutLogId == id && w.UserId == userId);

                if (workout == null)
                {
                    _logger.LogWarning("Workout {WorkoutId} not found for user {UserId}", id, userId);
                    return NotFound();
                }

                _context.WorkoutLogs.Remove(workout);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Workout {WorkoutId} deleted for user {UserId}", id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete workout {WorkoutId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogWorkout([FromBody] WorkoutLogCreateDTO dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();

                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);
                var userWeight = userProfile?.WeightKg ?? 70;

                int totalWorkoutCalories = 0;
                int totalDuration = 0;

                foreach (var exercise in dto.Exercises)
                {
                    if (exercise.CaloriesBurned != null && exercise.CaloriesBurned > 0)
                    {
                        totalWorkoutCalories += exercise.CaloriesBurned.Value;
                        totalDuration += exercise.DurationMin ?? 0;
                        continue;
                    }

                    var catalog = await _context.ExerciseCatalog
                        .FirstOrDefaultAsync(ec => ec.ExerciseCatalogId == exercise.ExerciseCatalogId);

                    if (catalog == null) continue;

                    if (exercise.DurationMin <= 0 && exercise.Sets > 0 && exercise.Reps > 0)
                    {
                        int totalReps = exercise.Reps.Value * exercise.Sets.Value;
                        double estimatedDuration = (totalReps * 2.5 + (exercise.Sets.Value - 1) * 30) / 60.0;
                        exercise.DurationMin = (int)Math.Round(estimatedDuration);
                    }

                    totalDuration += exercise.DurationMin ?? 0;

                    if (catalog.METValue.HasValue)
                    {
                        exercise.CaloriesBurned = CalorieHelper.EstimateCaloriesFromMet(
                            catalog.METValue.Value, exercise.DurationMin.Value, userWeight);
                    }
                    else
                    {
                        exercise.CaloriesBurned = CalorieHelper.EstimateCaloriesFromType(
                            catalog.Category, exercise.DurationMin.Value, userWeight);
                    }

                    totalWorkoutCalories += exercise.CaloriesBurned ?? 0;
                }

                var workoutLog = new WorkoutLog
                {
                    UserId = userId,
                    WorkoutTemplateId = dto.WorkoutTemplateId,
                    DurationMin = totalDuration,
                    CaloriesBurned = totalWorkoutCalories,
                    LoggedAt = dto.LoggedAt,
                    WorkoutLogExercises = dto.Exercises.Select(e => new WorkoutLogExercise
                    {
                        ExerciseCatalogId = e.ExerciseCatalogId,
                        Reps = e.Reps,
                        Sets = e.Sets,
                        WeightKg = e.WeightKg,
                        DurationMin = e.DurationMin,
                        CaloriesBurned = e.CaloriesBurned,
                        PersonalRecord = e.PersonalRecord,
                        Notes = e.Notes
                    }).ToList()
                };

                _context.WorkoutLogs.Add(workoutLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Workout logged successfully for user {UserId}. WorkoutLogId: {WorkoutLogId}", userId, workoutLog.WorkoutLogId);
                return Ok(new { workoutLog.WorkoutLogId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log workout for user {UserId}", GetUserIdFromClaims());
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetWorkoutSummary([FromQuery] DateTime date)
        {
            try
            {
                var userId = GetUserIdFromClaims();

                var totalCaloriesBurned = await _workoutRepo.GetTotalCaloriesBurnedForUserOnDateAsync(userId, date);

                _logger.LogInformation("Workout summary fetched for user {UserId} on {Date}", userId, date.Date);
                return Ok(new { totalCaloriesBurned });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch workout summary");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("range")]
        public async Task<IActionResult> GetWorkoutsInRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var userId = GetUserIdFromClaims();

                var workouts = await _context.WorkoutLogs
                    .Where(w => w.UserId == userId && w.LoggedAt >= start && w.LoggedAt <= end)
                    .Include(w => w.WorkoutLogExercises)
                        .ThenInclude(e => e.ExerciseCatalog)
                    .Include(w => w.WorkoutTemplate)
                    .OrderByDescending(w => w.LoggedAt)
                    .ToListAsync();

                var dtos = workouts.Select(w => new WorkoutLogReadDTO
                {
                    WorkoutLogId = w.WorkoutLogId,
                    LoggedAt = w.LoggedAt ?? DateTime.Now,
                    DurationMin = w.DurationMin ?? 0,
                    CaloriesBurned = w.CaloriesBurned ?? 0,
                    WorkoutTemplateName = w.WorkoutTemplate?.Name,
                    Exercises = w.WorkoutLogExercises.Select(e => new WorkoutLogExerciseReadDTO
                    {
                        ExerciseName = e.ExerciseCatalog?.Name ?? "Unknown",
                        Category = e.ExerciseCatalog?.Category ?? "Unknown",
                        Reps = e.Reps ?? 0,
                        Sets = e.Sets ?? 0,
                        WeightKg = (float?)e.WeightKg,
                        DurationMin = e.DurationMin ?? 0,
                        CaloriesBurned = e.CaloriesBurned,
                        PersonalRecord = e.PersonalRecord ?? false,
                        Notes = e.Notes ?? ""
                    }).ToList()
                });

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch workouts in date range");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        private int GetUserIdFromClaims()
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim missing in request");
                    return -1;
                }

                return int.Parse(userIdClaim.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse user ID from claims");
                return -99;
            }
        }
    }
}
