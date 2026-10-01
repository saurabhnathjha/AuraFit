using Microsoft.EntityFrameworkCore;
using AuraFitDataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraFitDataAccessLayer.Repositories.Interfaces;

namespace AuraFitDataAccessLayer.Repositories
{
    public class UserGoalRepository:IUserGoalRepository
    {
        private readonly AuraFitDbContext _context;

        public UserGoalRepository(AuraFitDbContext context)
        {
            _context = context;
        }

        public async Task<UserGoal?> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == userId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpsertAsync(UserGoal goal)
        {
            try
            {
                var existingGoal = await _context.UserGoals.FirstOrDefaultAsync(g => g.UserId == goal.UserId);
                if (existingGoal != null)
                {
                    existingGoal.GoalType = goal.GoalType;
                    existingGoal.WeeklyTargetKg = goal.WeeklyTargetKg;
                    existingGoal.TargetCalories = goal.TargetCalories;
                    existingGoal.AutoSuggested = goal.AutoSuggested;
                    existingGoal.GoalStartDate = goal.GoalStartDate;
                }
                else
                {
                    await _context.UserGoals.AddAsync(goal);
                }
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}

