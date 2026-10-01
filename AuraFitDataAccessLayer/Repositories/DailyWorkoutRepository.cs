using AuraFitDataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraFitDataAccessLayer.Repositories
{
    public class DailyWorkoutRepository
    {
        private readonly AuraFitDbContext _context;

        public DailyWorkoutRepository(AuraFitDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<DailyWorkout>> GetDailyWorkoutsAsync(int userId, string dayOfWeek)
        {
            return await _context.DailyWorkouts
                .Where(dw => dw.WeeklyWorkoutPlan.UserId == userId && dw.DayOfWeek == dayOfWeek)
                .Select(dw => new DailyWorkout
                {
                    ExerciseId = dw.ExerciseId,
                    DayOfWeek = dw.DayOfWeek
                })
                .ToListAsync();
        }

    }
}
