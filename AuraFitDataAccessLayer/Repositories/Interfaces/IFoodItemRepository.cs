using AuraFitDataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraFitDataAccessLayer.Repositories.Interfaces
{
    public interface IFoodItemRepository
    {
        Task<bool> AddFoodItemAsync(FoodItem item);
        Task<bool> AddFoodItemsAsync(List<FoodItem> items);
        Task<List<FoodItem>> GetFoodItemsByMealIdAsync(int mealId);
    }

}
