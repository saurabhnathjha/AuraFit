using AuraFitDataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraFitDataAccessLayer.Repositories.Interfaces
{
    public interface IWorkoutLogRepository
    {
        Task<IEnumerable<WorkoutLog>> GetAllLogsByUserAsync(int userId);
        Task<WorkoutLog?> GetLogByIdAsync(int logId);
        Task<WorkoutLog?> GetLogByIdAndUserAsync(int logId, int userId);
        Task<WorkoutLog> AddWorkoutLogAsync(WorkoutLog workoutLog);
        Task AddExerciseToLogAsync(WorkoutLogExercise logExercise);
        Task UpdateWorkoutLogAsync(WorkoutLog workoutLog, List<WorkoutLogExercise> updatedExercises);
        Task DeleteWorkoutLogAsync(WorkoutLog workoutLog);

        Task<int> GetTotalCaloriesBurnedForUserOnDateAsync(int userId, DateTime date);
    }


}
