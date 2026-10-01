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
    public class FoodItemRepository : IFoodItemRepository
    {
        private readonly AuraFitDbContext _context;

        public FoodItemRepository(AuraFitDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddFoodItemAsync(FoodItem item)
        {
            try
            {
                _context.FoodItems.Add(item);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddFoodItemsAsync(List<FoodItem> items)
        {
            try
            {
                _context.FoodItems.AddRange(items);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<FoodItem>> GetFoodItemsByMealIdAsync(int mealId)
        {
            try
            {
                return await _context.FoodItems
                    .Where(f => f.MealId == mealId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }

        }
    }

}
