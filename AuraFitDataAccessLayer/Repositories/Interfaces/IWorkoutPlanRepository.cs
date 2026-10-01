using AuraFitDataAccessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuraFitDataAccessLayer.Repositories.Interfaces
{
    public interface IWorkoutPlanRepository
    {
        Task<WeeklyWorkoutPlan> CreateWorkoutPlanAsync(WeeklyWorkoutPlan plan);
        Task<List<DailyWorkout>> GetWorkoutsByDayAsync(int planId, string dayOfWeek);
        Task<List<DailyWorkout>> GetAllDailyWorkoutsAsync(int planId);
        Task<IEnumerable<DailyWorkout>> GetDailyWorkoutsAsync(int userId, string dayOfWeek);
        Task<bool> DeleteDailyWorkoutAsync(int userId, int exerciseId, string dayOfWeek);
        Task<bool> AddMultipleDailyWorkoutsAsync(List<DailyWorkout> dailyWorkouts);

        Task<WeeklyWorkoutPlan?> GetPlanByUserIdAsync(int userId);

    }
}
