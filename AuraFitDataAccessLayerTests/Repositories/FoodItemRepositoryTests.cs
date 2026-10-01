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
    public class FoodItemRepositoryTests
    {
        private AuraFitDbContext _context;
        private FoodItemRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuraFitDbContext>()
                .UseInMemoryDatabase(databaseName: "FoodItemTestDb")
                .Options;

            _context = new AuraFitDbContext(options);
            _repository = new FoodItemRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task AddFoodItemAsync_ShouldAddItem()
        {
            var item = new FoodItem
            {
                FoodItemId = 1,
                Name = "Boiled Egg",
                MealId = 100,
                Quantity = "2 pieces",
                Calories = 150,
                Source = "User"
            };

            var result = await _repository.AddFoodItemAsync(item);

            Assert.IsTrue(result);
            Assert.AreEqual(1, _context.FoodItems.Count());
            Assert.AreEqual("Boiled Egg", _context.FoodItems.First().Name);
        }

        [TestMethod]
        public async Task AddFoodItemsAsync_ShouldAddMultipleItems()
        {
            var items = new List<FoodItem>
            {
                new FoodItem { FoodItemId = 2, Name = "Brown Rice", MealId = 101, Quantity = "1 bowl", Calories = 210, Source = "System" },
                new FoodItem { FoodItemId = 3, Name = "Grilled Chicken", MealId = 101, Quantity = "150g", Calories = 280, Source = "System" }
            };

            var result = await _repository.AddFoodItemsAsync(items);

            Assert.IsTrue(result);
            Assert.AreEqual(2, _context.FoodItems.Count());
        }

        [TestMethod]
        public async Task GetFoodItemsByMealIdAsync_ShouldReturnItemsWithMatchingMealId()
        {
            var items = new List<FoodItem>
            {
                new FoodItem { FoodItemId = 4, Name = "Chapati", MealId = 201, Quantity = "2", Calories = 200, Source = "User" },
                new FoodItem { FoodItemId = 5, Name = "Paneer", MealId = 201, Quantity = "100g", Calories = 250, Source = "User" },
                new FoodItem { FoodItemId = 6, Name = "Salad", MealId = 202, Quantity = "1 plate", Calories = 50, Source = "User" }
            };

            await _context.FoodItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            var result = await _repository.GetFoodItemsByMealIdAsync(201);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(item => item.MealId == 201));
        }
    }
}





       
    

