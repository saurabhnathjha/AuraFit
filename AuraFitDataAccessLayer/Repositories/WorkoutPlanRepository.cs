using AuraFitDataAccessLayer.Models;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuraFitDataAccessLayer.Repositories
{
    public class WorkoutPlanRepository : IWorkoutPlanRepository
    {
        private readonly AuraFitDbContext _context;

        public WorkoutPlanRepository(AuraFitDbContext context)
        {
            _context = context;
        }

        public async Task<WeeklyWorkoutPlan> CreateWorkoutPlanAsync(WeeklyWorkoutPlan plan)
        {
            _context.WeeklyWorkoutPlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<List<DailyWorkout>> GetWorkoutsByDayAsync(int planId, string dayOfWeek)
        {
            return await _context.DailyWorkouts
                .Where(dw => dw.PlanId == planId && dw.DayOfWeek == dayOfWeek)
                .ToListAsync();
        }

        public async Task<List<DailyWorkout>> GetAllDailyWorkoutsAsync(int planId)
        {
            return await _context.DailyWorkouts
                .Where(dw => dw.PlanId == planId)
                .ToListAsync();
        }
        public async Task<IEnumerable<DailyWorkout>> GetDailyWorkoutsAsync(int userId, string dayOfWeek)
        {
            return await _context.DailyWorkouts
                .Include(dw => dw.WeeklyWorkoutPlan) // valid navigation include
                .Where(dw => dw.WeeklyWorkoutPlan.UserId == userId && dw.DayOfWeek == dayOfWeek)
                .ToListAsync();
        }
        public async Task<bool> DeleteDailyWorkoutAsync(int userId, int exerciseId, string dayOfWeek)
        {
            // Get all PlanIds of this user (supporting multiple plans)
            var planIds = await _context.WeeklyWorkoutPlans
                .Where(p => p.UserId == userId)
                .Select(p => p.PlanId)
                .ToListAsync();

            if (!planIds.Any())
                return false;

            // Find all matching workouts
            var workouts = await _context.DailyWorkouts
                .Where(dw => planIds.Contains(dw.PlanId) &&
                             dw.ExerciseId == exerciseId &&
                             dw.DayOfWeek == dayOfWeek)
                .ToListAsync();

            if (!workouts.Any())
                return false;

            _context.DailyWorkouts.RemoveRange(workouts);
            await _context.SaveChangesAsync();

            return true;

        }
       
        public async Task<WeeklyWorkoutPlan?> GetPlanByUserIdAsync(int userId)
        {
            return await _context.WeeklyWorkoutPlans
                                 .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<bool> AddMultipleDailyWorkoutsAsync(List<DailyWorkout> workouts)
        {
            await _context.DailyWorkouts.AddRangeAsync(workouts);
            return await _context.SaveChangesAsync() > 0;
        }




    }
}
