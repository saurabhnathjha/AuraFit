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
    public class MealRepository : IMealRepository
    {
        private readonly AuraFitDbContext _context;

        public MealRepository(AuraFitDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateMealAsync(Meal meal)
        {
            try
            {
                _context.Meals.Add(meal);
                await _context.SaveChangesAsync();
                return meal.MealId;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        public async Task<Meal?> GetMealWithItemsAsync(int mealId)
        {
            try
            {
                return await _context.Meals
                   .Include(m => m.FoodItems)
                   .FirstOrDefaultAsync(m => m.MealId == mealId);
            }
            catch (Exception)
            {
                return null;
            }
       
        }

        public async Task<List<Meal>> GetMealsByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Meals
                    .Where(m => m.UserId == userId)
                    .Include(m => m.FoodItems)
                    .OrderByDescending(m => m.LoggedAt)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<int> GetTotalCaloriesForUserOnDateAsync(int userId, DateTime date)
        {
            try
            {
                var totalCalories = await _context.Meals
                    .Where(m => m.UserId == userId && m.LoggedAt.HasValue && m.LoggedAt.Value.Date == date.Date)
                    .SelectMany(m => m.FoodItems)
                    .SumAsync(f => (int?)f.Calories) ?? 0;
                return totalCalories;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<bool> UpdateMealAsync(int userId, int mealId, Meal updatedMeal)
        {
            try
            {
                var existingMeal = await _context.Meals
                    .Include(m => m.FoodItems)
                    .FirstOrDefaultAsync(m => m.MealId == mealId && m.UserId == userId);

                if (existingMeal == null)
                    return false;

                // Clear old food items
                _context.FoodItems.RemoveRange(existingMeal.FoodItems);

                // Update core meal fields
                existingMeal.Name = updatedMeal.Name;
                existingMeal.Description = updatedMeal.Description;
                existingMeal.LoggedAt = DateTime.UtcNow;
                existingMeal.TotalCalories = updatedMeal.TotalCalories;
                existingMeal.Source = updatedMeal.Source;

                // Add new food items
                foreach (var item in updatedMeal.FoodItems)
                {
                    item.MealId = mealId; // ensure correct linkage
                    _context.FoodItems.Add(item);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteMealAsync(int userId, int mealId)
        {
            try
            {
                var meal = await _context.Meals
                    .Include(m => m.FoodItems)
                    .FirstOrDefaultAsync(m => m.MealId == mealId && m.UserId == userId);

                if (meal == null)
                    return false;

                _context.FoodItems.RemoveRange(meal.FoodItems);
                _context.Meals.Remove(meal);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }

}
