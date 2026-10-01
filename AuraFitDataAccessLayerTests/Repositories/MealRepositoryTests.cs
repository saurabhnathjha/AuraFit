using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuraFitDataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraFitDataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuraFitDataAccessLayer.Repositories.Tests
{
    [TestClass()]
    public class MealRepositoryTests
    {



        private AuraFitDbContext _context;
        private MealRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuraFitDbContext>()
                .UseInMemoryDatabase(databaseName: "MealTestDb")
                .Options;

            _context = new AuraFitDbContext(options);
            _repository = new MealRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task CreateMealAsyncTest()
        {
            var meal = new Meal
            {
                UserId = 1,
                Name = "Lunch",
                Description = "Simple lunch",
                LoggedAt = DateTime.UtcNow,
                Source = "User",
                TotalCalories = 500
            };

            var result = await _repository.CreateMealAsync(meal);

            Assert.IsTrue(result > 0);
            Assert.AreEqual(1, _context.Meals.Count());
        }

        [TestMethod]
        public async Task GetMealWithItemsAsyncTest()
        {
            var meal = new Meal
            {
                UserId = 1,
                Name = "Dinner",
                LoggedAt = DateTime.UtcNow,
                FoodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Rice", Quantity = "1 cup", Calories = 200, Source = "User" },
                    new FoodItem { Name = "Dal", Quantity = "1 bowl", Calories = 150, Source = "User" }
                }
            };

            _context.Meals.Add(meal);
            await _context.SaveChangesAsync();

            var result = await _repository.GetMealWithItemsAsync(meal.MealId);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.FoodItems.Count);
        }

        [TestMethod]
        public async Task GetMealsByUserIdAsyncTest()
        {
            var meals = new List<Meal>
            {
                new Meal { UserId = 2, Name = "Breakfast", LoggedAt = DateTime.UtcNow.AddHours(-5) },
                new Meal { UserId = 2, Name = "Lunch", LoggedAt = DateTime.UtcNow }
            };

            await _context.Meals.AddRangeAsync(meals);
            await _context.SaveChangesAsync();

            var result = await _repository.GetMealsByUserIdAsync(2);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Lunch", result.First().Name);
        }

        [TestMethod]
        public async Task GetTotalCaloriesForUserOnDateAsyncTest()
        {
            var today = DateTime.UtcNow.Date;

            var meal = new Meal
            {
                UserId = 3,
                Name = "Test Meal", // Required property added
                LoggedAt = today,
                FoodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Paneer", Calories = 300 },
                    new FoodItem { Name = "Salad", Calories = 100 }
                }
            };

            _context.Meals.Add(meal);
            await _context.SaveChangesAsync();

            var result = await _repository.GetTotalCaloriesForUserOnDateAsync(3, today);

            Assert.AreEqual(400, result);
        }

        [TestMethod]
        public async Task UpdateMealAsyncTest()
        {
            var meal = new Meal
            {
                UserId = 4,
                Name = "Old Meal",
                LoggedAt = DateTime.UtcNow,
                FoodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Roti", Calories = 100 }
                }
            };

            await _context.Meals.AddAsync(meal);
            await _context.SaveChangesAsync();

            var updatedMeal = new Meal
            {
                Name = "Updated Meal",
                Description = "Updated desc",
                TotalCalories = 600,
                Source = "System",
                FoodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Bread", Calories = 250 },
                    new FoodItem { Name = "Butter", Calories = 100 }
                }
            };

            var result = await _repository.UpdateMealAsync(4, meal.MealId, updatedMeal);

            Assert.IsTrue(result);
            var updated = await _context.Meals.Include(m => m.FoodItems).FirstOrDefaultAsync(m => m.MealId == meal.MealId);
            Assert.AreEqual("Updated Meal", updated.Name);
            Assert.AreEqual(2, updated.FoodItems.Count);
        }

        [TestMethod]
        public async Task DeleteMealAsyncTest()
        {
            var meal = new Meal
            {
                UserId = 5,
                Name = "Test Meal",
                FoodItems = new List<FoodItem>
                {
                    new FoodItem { Name = "Egg", Calories = 70 },
                    new FoodItem { Name = "Toast", Calories = 90 }
                }
            };

            await _context.Meals.AddAsync(meal);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteMealAsync(5, meal.MealId);

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.Meals.Count());
            Assert.AreEqual(0, _context.FoodItems.Count());
        }
    }
}
