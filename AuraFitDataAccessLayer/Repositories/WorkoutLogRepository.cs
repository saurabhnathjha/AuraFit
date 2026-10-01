using Microsoft.EntityFrameworkCore;
using AuraFitDataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuraFitDataAccessLayer.Repositories.Interfaces;

namespace AuraFitDataAccessLayer.Repositories
{
    public class WorkoutLogRepository : IWorkoutLogRepository
    {
        private readonly AuraFitDbContext _context;

        public WorkoutLogRepository(AuraFitDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkoutLog>> GetAllLogsByUserAsync(int userId)
        {
            try
            {
                return await _context.WorkoutLogs
                 .Include(w => w.WorkoutLogExercises)
                 .Where(w => w.UserId == userId)
                 .OrderByDescending(w => w.LoggedAt)
                 .ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<WorkoutLog?> GetLogByIdAsync(int logId)
        {
            try
            {
                return await _context.WorkoutLogs
                    .Include(w => w.WorkoutLogExercises)
                    .FirstOrDefaultAsync(w => w.WorkoutLogId == logId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<WorkoutLog?> GetLogByIdAndUserAsync(int logId, int userId)
        {
            try
            {
                return await _context.WorkoutLogs
                    .Include(w => w.WorkoutLogExercises)
                    .FirstOrDefaultAsync(w => w.WorkoutLogId == logId && w.UserId == userId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<WorkoutLog> AddWorkoutLogAsync(WorkoutLog workoutLog)
        {
            try
            {
                //Change
                var userExists = await _context.Users.AnyAsync(u => u.UserId == workoutLog.UserId);
                if (!userExists) return null;

                _context.WorkoutLogs.Add(workoutLog);
                await _context.SaveChangesAsync();
                return workoutLog;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task AddExerciseToLogAsync(WorkoutLogExercise logExercise)
        {
                _context.WorkoutLogExercises.Add(logExercise);
                await _context.SaveChangesAsync();

        }

        public async Task UpdateWorkoutLogAsync(WorkoutLog workoutLog, List<WorkoutLogExercise> updatedExercises)
        {

                // Remove existing exercises
                var existingExercises = _context.WorkoutLogExercises.Where(e => e.WorkoutLogId == workoutLog.WorkoutLogId);
                _context.WorkoutLogExercises.RemoveRange(existingExercises);

                // Add updated exercises
                workoutLog.WorkoutLogExercises = updatedExercises;
                _context.WorkoutLogs.Update(workoutLog);

                await _context.SaveChangesAsync();

        }

        public async Task<int> GetTotalCaloriesBurnedForUserOnDateAsync(int userId, DateTime date)
        {
            try
            {
                var totalBurned = await _context.WorkoutLogs
                    .Where(w => w.UserId == userId && w.LoggedAt.HasValue && w.LoggedAt.Value.Date == date.Date)
                    .SumAsync(w => (int?)w.CaloriesBurned) ?? 0;

                return totalBurned;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public async Task DeleteWorkoutLogAsync(WorkoutLog workoutLog)
        {

                _context.WorkoutLogs.Remove(workoutLog);
                await _context.SaveChangesAsync();

        }


    }
}
