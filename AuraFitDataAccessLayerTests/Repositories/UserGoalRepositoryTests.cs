#region testing



//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using AuraFitDataAccessLayer.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AuraFitDataAccessLayer.Repositories.Tests
//{
//    [TestClass()]
//    public class UserGoalRepositoryTests
//    {
//        [TestMethod()]
//        public void UserGoalRepositoryTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void GetByUserIdAsyncTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void UpsertAsyncTest()
//        {
//            Assert.Fail();
//        }
//    }
//}
#endregion



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
    public class UserGoalRepositoryTests
    {
        private AuraFitDbContext _context;
        private UserGoalRepository _repository;
        [TestInitialize]
        public void setup()
        {
            // creating a temp db instead of real sql server
            var options = new DbContextOptionsBuilder<AuraFitDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;

            // Initializing the db with the temp db
            _context = new AuraFitDbContext(options);


            _repository = new UserGoalRepository(_context);
        }

        [TestMethod]
        public async Task GetByUserIdAsync_ReturnGoalExists()
        {
            var goal = new UserGoal
            {
                UserId = 1,
                GoalType = "LoseWeight",
                WeeklyTargetKg = 0.5,
                TargetCalories = 2200,
                AutoSuggested = true,
                GoalStartDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _context.UserGoals.AddAsync(goal);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByUserIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2200, result.TargetCalories);
        }



        [TestMethod]
        public async Task GetByUserIdAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _repository.GetByUserIdAsync(99);

            // Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task GetByUserIdAsync_ReturnsNull_OnException()
        {


            // Act
            var result = await _repository.GetByUserIdAsync(1);

            // Assert
            Assert.IsNull(result); // it hits catch block
        }

        [TestMethod]
        public async Task UpsertAsync_UpdatesExistingGoal_ReturnsTrue()
        {
            // Arrange: Add initial goal
            var initialGoal = new UserGoal
            {
                UserId = 1,
                GoalType = "LoseWeight",
                WeeklyTargetKg = 1,
                TargetCalories = 2000,
                AutoSuggested = true,
                GoalStartDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            _context.UserGoals.Add(initialGoal);
            await _context.SaveChangesAsync();

            // Act: Modify and call Upsert
            var updatedGoal = new UserGoal
            {
                UserId = 1,
                GoalType = "GainWeight", // changed
                WeeklyTargetKg = 2,
                TargetCalories = 2500,
                AutoSuggested = false,
                GoalStartDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            var result = await _repository.UpsertAsync(updatedGoal);

            // Assert
            Assert.IsTrue(result);

            var goalInDb = await _repository.GetByUserIdAsync(1);
            Assert.AreEqual("GainWeight", goalInDb.GoalType);
            Assert.AreEqual(false, goalInDb.AutoSuggested);
        }

        [TestMethod]
        public async Task UpsertAsync_InsertsNewGoal_ReturnsTrue()
        {
            // Arrange
            var newGoal = new UserGoal
            {
                UserId = 2,
                GoalType = "Maintain",
                WeeklyTargetKg = 0,
                TargetCalories = 2200,
                AutoSuggested = false,
                GoalStartDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            // Act
            var result = await _repository.UpsertAsync(newGoal);

            // Assert
            Assert.IsTrue(result);
            var goalInDb = await _repository.GetByUserIdAsync(2);
            Assert.IsNotNull(goalInDb);
        }


        [TestMethod]
        public async Task UpsertAsync_AutoSuggestedIsFalse_SavesCorrectly()
        {
            var goal = new UserGoal
            {
                UserId = 3,
                GoalType = "GainWeight",
                WeeklyTargetKg = 1,
                TargetCalories = 1900,
                AutoSuggested = false, // key part
                GoalStartDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _repository.UpsertAsync(goal);

            var result = await _repository.GetByUserIdAsync(3);
            Assert.IsFalse(result.AutoSuggested);







        }

        [TestMethod]
        public async Task UpsertAsync_WhenExceptionThrown_ReturnsFalse()
        {
            _context.Dispose(); // force failure

            var repo = new UserGoalRepository(_context);

            var goal = new UserGoal
            {
                UserId = 99,
                GoalType = "LoseWeight",
                WeeklyTargetKg = 1,
                TargetCalories = 1000,
                AutoSuggested = true,
                GoalStartDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var result = await repo.UpsertAsync(goal);

            Assert.IsFalse(result);
        }
    }

}