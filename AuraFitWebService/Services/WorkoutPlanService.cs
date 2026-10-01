using AuraFitDataAccessLayer.Models;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using AuraFitWebServices.DTOs;
using AuraFitWebServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuraFitWebServices.Services
{
    public class WorkoutPlanService : IWorkoutPlanService
    {
        private readonly IWorkoutPlanRepository _repository;

        public WorkoutPlanService(IWorkoutPlanRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CreateWorkoutPlanAsync(CreateWorkoutPlanDTO planDto)
        {
            var plan = new WeeklyWorkoutPlan
            {
                UserId = planDto.UserId,
                PlanName = planDto.PlanName,
                DailyWorkouts = new List<DailyWorkout>()
            };

            foreach (var daily in planDto.DailyWorkouts)
            {
                plan.DailyWorkouts.Add(new DailyWorkout
                {
                    DayOfWeek = daily.DayOfWeek,
                    ExerciseId = daily.ExerciseId
                });
            }

            var created = await _repository.CreateWorkoutPlanAsync(plan);
            return created.PlanId;
        }
        public async Task<IEnumerable<DailyWorkoutDTO>> GetDailyWorkoutAsync(int userId, string dayOfWeek)
        {
            var workouts = await _repository.GetDailyWorkoutsAsync(userId, dayOfWeek);

            return workouts.Select(w => new DailyWorkoutDTO
            {
                ExerciseId = w.ExerciseId,
                DayOfWeek = w.DayOfWeek
            });
        }
        public async Task<bool> DeleteDailyWorkoutAsync(int userId, int exerciseId, string dayOfWeek)
        {
            return await _repository.DeleteDailyWorkoutAsync(userId, exerciseId, dayOfWeek);
        }
        //public async Task<bool> AddMultipleDailyWorkoutsAsync(AddDailyWorkoutDTO dto)
        //{
        //    // 1. Fetch the WeeklyWorkoutPlan using UserId
        //    var plan = await _repository.GetPlanByUserIdAsync(dto.UserId);
        //    if (plan == null)
        //        return false;

        //    // 2. Create DailyWorkout entries with PlanId
        //    var dailyWorkouts = dto.ExerciseIds.Select(eid => new DailyWorkout
        //    {
        //        PlanId = plan.PlanId,
        //        DayOfWeek = dto.DayOfWeek,
        //        ExerciseId = eid
        //    }).ToList();

        //    // 3. Save to DB
        //    return await _repository.AddMultipleDailyWorkoutsAsync(dailyWorkouts);
        //}

        //public async Task<bool> AddMultipleDailyWorkoutsAsync(List<AddDailyWorkoutDTO> workouts)
        //{
        //    if (workouts == null || !workouts.Any())
        //        return false;

        //    int userId = workouts.First().UserId;
        //    var plan = await _repository.GetPlanByUserIdAsync(userId);
        //    if (plan == null)
        //        return false;

        //    var dailyWorkouts = new List<DailyWorkout>();

        //    foreach (var w in workouts)
        //    {
        //        foreach (var exerciseId in w.ExerciseIds)
        //        {
        //            dailyWorkouts.Add(new DailyWorkout
        //            {
        //                PlanId = plan.PlanId,
        //                DayOfWeek = w.DayOfWeek,
        //                ExerciseId = exerciseId
        //            });
        //        }
        //    }

        //    return await _repository.AddMultipleDailyWorkoutsAsync(dailyWorkouts);
        //}
        public async Task<bool> AddMultipleDailyWorkoutsAsync(AddDailyWorkoutDTO dto)
        {
            if (dto == null || dto.ExerciseIds == null || !dto.ExerciseIds.Any())
                return false;

            var plan = await _repository.GetPlanByUserIdAsync(dto.UserId);
            if (plan == null)
                return false;

            // Step 1: Get existing ExerciseIds for the given day
            var existingWorkouts = await _repository.GetDailyWorkoutsAsync(dto.UserId, dto.DayOfWeek);
            var existingExerciseIds = existingWorkouts.Select(e => e.ExerciseId).ToHashSet();

            // Step 2: Filter out duplicates
            var newExerciseIds = dto.ExerciseIds
                .Where(id => !existingExerciseIds.Contains(id))
                .Distinct() // avoid duplicates within payload itself
                .ToList();

            if (!newExerciseIds.Any())
                return false;

            // Step 3: Create new DailyWorkout entries
            var dailyWorkouts = newExerciseIds.Select(id => new DailyWorkout
            {
                PlanId = plan.PlanId,
                DayOfWeek = dto.DayOfWeek,
                ExerciseId = id
            }).ToList();

            return await _repository.AddMultipleDailyWorkoutsAsync(dailyWorkouts);
        }

        public async Task<bool> DoesPlanExistForUserAsync(int userId)
        {
            var plan = await _repository.GetPlanByUserIdAsync(userId);
            return plan != null;
        }




    }
}
