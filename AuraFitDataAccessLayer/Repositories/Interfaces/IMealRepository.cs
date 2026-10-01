using AuraFitDataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraFitDataAccessLayer.Repositories.Interfaces
{
    public interface IMealRepository
    {
        Task<int> CreateMealAsync(Meal meal); // Returns MealId
        Task<Meal?> GetMealWithItemsAsync(int mealId);
        Task<List<Meal>> GetMealsByUserIdAsync(int userId);

        Task<int> GetTotalCaloriesForUserOnDateAsync(int userId, DateTime date);

        Task<bool> UpdateMealAsync(int userId, int mealId, Meal updatedMeal);
        Task<bool> DeleteMealAsync(int userId, int mealId);

    }
}
