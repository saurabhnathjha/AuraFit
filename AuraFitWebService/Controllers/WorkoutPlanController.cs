using AuraFitWebService.Controllers;
using AuraFitWebService.DTOs;
using AuraFitWebServices.DTOs;
using AuraFitWebServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuraFitWebServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutPlanController : ControllerBase
    {
        private readonly IWorkoutPlanService _workoutPlanService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<WorkoutLogController> _logger;

        public WorkoutPlanController(IHttpContextAccessor httpContextAccessor, IWorkoutPlanService workoutPlanService, ILogger<WorkoutLogController> logger)
        {
            _workoutPlanService = workoutPlanService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkoutPlan([FromBody] CreateWorkoutPlanDTO planDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var planId = await _workoutPlanService.CreateWorkoutPlanAsync(planDto);
            return Ok(new { PlanId = planId });
        }
        [HttpGet("daily-plan")]
        public async Task<IActionResult> GetDailyWorkout([FromQuery] int userId, [FromQuery] DateTime date)
        {
            
            var workouts = await _workoutPlanService.GetDailyWorkoutAsync(userId, date.DayOfWeek.ToString());

            if (workouts == null || !workouts.Any())
                return NotFound("No workout plan found for today.");

            return Ok(workouts);
        }
        //[HttpDelete("daily-plan/{userId}/{exerciseId}")]
        //public async Task<IActionResult> DeleteDailyWorkout(int userId, int exerciseId)
        //{
        //    var success = await _workoutPlanService.DeleteDailyWorkoutAsync(userId, exerciseId);
        //    if (!success)
        //        return NotFound("Exercise not found or already removed.");

        //    return Ok("Exercise removed successfully.");
        //}
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDailyWorkout([FromQuery] DeleteDailyWorkoutDTO dto)
        {
            var success = await _workoutPlanService.DeleteDailyWorkoutAsync(dto.UserId, dto.ExerciseId, dto.DayOfWeek);

            //if (!success)
            //    return NotFound("Workout not found for the given details.");

            //return Ok("Workout deleted successfully.");
            if (success)
                return NoContent(); // 204 No Content – clean, recommended
            else
                return NotFound();
        }
        [HttpPost("add-multiple")]
        public async Task<IActionResult> AddMultipleDailyWorkouts([FromBody] AddDailyWorkoutDTO dto)
        {
            var result = await _workoutPlanService.AddMultipleDailyWorkoutsAsync(dto);
            if (!result) return BadRequest("Failed to add workouts.");
            return Ok("Workouts added successfully.");
        }

        [HttpGet("exists/{userId}")]
        public async Task<ActionResult<bool>> DoesPlanExist(int userId)
        {
            var uid = GetUserIdFromClaims();
            //bool exists = await _workoutPlanService.DoesPlanExistForUserAsync(uid);

            bool exists = await _workoutPlanService.DoesPlanExistForUserAsync(userId);
            return Ok(exists);
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
