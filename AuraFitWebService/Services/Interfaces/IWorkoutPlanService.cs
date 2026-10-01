using AuraFitWebServices.DTOs;
using System.Threading.Tasks;

namespace AuraFitWebServices.Services.Interfaces
{
    public interface IWorkoutPlanService
    {
        Task<int> CreateWorkoutPlanAsync(CreateWorkoutPlanDTO planDto);
        Task<IEnumerable<DailyWorkoutDTO>> GetDailyWorkoutAsync(int userId, string dayOfWeek);
        Task<bool> DeleteDailyWorkoutAsync(int userId, int exerciseId, string dayOfWeek);

        Task<bool> AddMultipleDailyWorkoutsAsync(AddDailyWorkoutDTO workouts);
        Task<bool> DoesPlanExistForUserAsync(int userId);

    }
}
