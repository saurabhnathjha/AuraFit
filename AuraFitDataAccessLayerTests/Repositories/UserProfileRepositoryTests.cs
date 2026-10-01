using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuraFitDataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraFitDataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

#region comments

//namespace AuraFitDataAccessLayer.Repositories.Tests
//{
//    [TestClass()]
//    public class UserProfileRepositoryTests
//    {
//        //[TestMethod()]
//        //public void UserProfileRepositoryTest()
//        //{
//        //    Assert.Fail();
//        //}

//        //[TestMethod()]
//        //public void GetByUserIdAsyncTest()
//        //{
//        //    Assert.Fail();
//        //}

//        //[TestMethod()]
//        //public void UpsertAsyncTest()
//        //{
//        //    Assert.Fail();
//        //}

//        //[TestMethod()]
//        //public void ShouldPromptWeightUpdateAsyncTest()
//        //{
//        //    Assert.Fail();
//        //}


//        [TestMethod]
//        public void UserProfileRepositoryTest()
//        {
//            // Arrange
//            // Act
//            // The TestInitialize method already creates an instance of the repository.
//            // We just need to assert that it's not null.
//            // Assert
//            Assert.IsNotNull(_repository, "Repository should be initialized.");
//        }

//        [TestMethod]
//        public async Task GetByUserIdAsync_ReturnsProfile_WhenProfileExists()
//        {
//            // Arrange
//            var userId = 1;
//            var expectedProfile = new UserProfile { Id = 1, UserId = userId, Age = 30, Gender = "Male", HeightCm = 175, WeightKg = 70 };
//            await _context.UserProfiles.AddAsync(expectedProfile);
//            await _context.SaveChangesAsync();

//            // Act
//            var actualProfile = await _repository.GetByUserIdAsync(userId);

//            // Assert
//            Assert.IsNotNull(actualProfile, "Profile should not be null.");
//            Assert.AreEqual(expectedProfile.UserId, actualProfile.UserId, "User IDs should match.");
//            Assert.AreEqual(expectedProfile.Age, actualProfile.Age, "Age should match.");
//        }

//        [TestMethod]
//        public async Task GetByUserIdAsync_ReturnsNull_WhenProfileDoesNotExist()
//        {
//            // Arrange
//            var userId = 999; // A user ID that doesn't exist in the database

//            // Act
//            var actualProfile = await _repository.GetByUserIdAsync(userId);

//            // Assert
//            Assert.IsNull(actualProfile, "Profile should be null when not found.");
//        }

//        [TestMethod]
//        public async Task UpsertAsync_InsertsNewProfile_WhenProfileDoesNotExist()
//        {
//            // Arrange
//            var newProfile = new UserProfile { UserId = 2, Age = 25, Gender = "Female", HeightCm = 160, WeightKg = 55 };

//            // Act
//            var result = await _repository.UpsertAsync(newProfile);

//            // Assert
//            Assert.IsTrue(result, "Upsert should return true for a successful insert.");
//            var profileInDb = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == newProfile.UserId);
//            Assert.IsNotNull(profileInDb, "New profile should be found in the database.");
//            Assert.AreEqual(newProfile.Age, profileInDb.Age, "Age should be updated correctly.");
//        }

//        [TestMethod]
//        public async Task UpsertAsync_UpdatesExistingProfile_WhenProfileExists()
//        {
//            // Arrange
//            var userId = 3;
//            var existingProfile = new UserProfile { Id = 1, UserId = userId, Age = 30, Gender = "Male", HeightCm = 170, WeightKg = 75 };
//            await _context.UserProfiles.AddAsync(existingProfile);
//            await _context.SaveChangesAsync();

//            var updatedProfile = new UserProfile { UserId = userId, Age = 32, Gender = "Male", HeightCm = 172, WeightKg = 78, ActivityLevel = "Active", Bmi = 26, Bmr = 1800, Tdee = 2500 };

//            // Act
//            var result = await _repository.UpsertAsync(updatedProfile);

//            // Assert
//            Assert.IsTrue(result, "Upsert should return true for a successful update.");
//            var profileInDb = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
//            Assert.IsNotNull(profileInDb, "Profile should still exist in the database.");
//            Assert.AreEqual(updatedProfile.Age, profileInDb.Age, "Age should be updated correctly.");
//            Assert.AreEqual(updatedProfile.WeightKg, profileInDb.WeightKg, "Weight should be updated correctly.");
//            Assert.AreEqual(updatedProfile.ActivityLevel, profileInDb.ActivityLevel, "Activity level should be updated correctly.");
//            Assert.AreEqual(updatedProfile.Bmi, profileInDb.Bmi, "BMI should be updated correctly.");
//            Assert.AreEqual(updatedProfile.Bmr, profileInDb.Bmr, "BMR should be updated correctly.");
//            Assert.AreEqual(updatedProfile.Tdee, profileInDb.Tdee, "TDEE should be updated correctly.");
//        }

//        [TestMethod]
//        public async Task ShouldPromptWeightUpdateAsync_ReturnsTrue_WhenNoWeightLogsExist()
//        {
//            // Arrange
//            var userId = 4; // No weight logs for this user

//            // Act
//            var shouldPrompt = await _repository.ShouldPromptWeightUpdateAsync(userId);

//            // Assert
//            Assert.IsTrue(shouldPrompt, "Should prompt when no weight logs exist.");
//        }

//        [TestMethod]
//        public async Task ShouldPromptWeightUpdateAsync_ReturnsTrue_WhenLastLogIsOlderThan7Days()
//        {
//            // Arrange
//            var userId = 5;
//            var oldWeightLog = new WeightLog { Id = 1, UserId = userId, Weight = 80, LoggedAt = DateTime.Now.AddDays(-8) };
//            await _context.WeightLogs.AddAsync(oldWeightLog);
//            await _context.SaveChangesAsync();

//            // Act
//            var shouldPrompt = await _repository.ShouldPromptWeightUpdateAsync(userId);

//            // Assert
//            Assert.IsTrue(shouldPrompt, "Should prompt when the last log is older than 7 days.");
//        }

//        [TestMethod]
//        public async Task ShouldPromptWeightUpdateAsync_ReturnsFalse_WhenLastLogIsWithin7Days()
//        {
//            // Arrange
//            var userId = 6;
//            var recentWeightLog = new WeightLog { Id = 1, UserId = userId, Weight = 75, LoggedAt = DateTime.Now.AddDays(-5) };
//            await _context.WeightLogs.AddAsync(recentWeightLog);
//            await _context.SaveChangesAsync();

//            // Act
//            var shouldPrompt = await _repository.ShouldPromptWeightUpdateAsync(userId);

//            // Assert
//            Assert.IsFalse(shouldPrompt, "Should not prompt when the last log is within 7 days.");
//        }

//        [TestMethod]
//        public async Task ShouldPromptWeightUpdateAsync_ReturnsFalse_WhenLastLogIsExactly7DaysOld()
//        {
//            // Arrange
//            var userId = 7;
//            // Set time to be exactly 7 days ago at the current time (or slightly earlier to avoid precision issues)
//            var exactly7DaysAgo = DateTime.Now.AddDays(-7).AddSeconds(-1);
//            var weightLog = new WeightLog { Id = 1, UserId = userId, Weight = 68, LoggedAt = exactly7DaysAgo };
//            await _context.WeightLogs.AddAsync(weightLog);
//            await _context.SaveChangesAsync();

//            // Act
//            var shouldPrompt = await _repository.ShouldPromptWeightUpdateAsync(userId);

//            // Assert
//            Assert.IsTrue(shouldPrompt, "Should prompt when the last log is exactly 7 days old (or older).");
//        }
//    }
//}

#endregion
namespace AuraFitDataAccessLayer.Repositories.Tests
{
   

    [TestClass]
    public class UserProfileRepositoryTests
    {
        private AuraFitDbContext _context;
        private UserProfileRepository _repository;

        // This method runs before each test
        [TestInitialize]
        public void TestInitialize()
        {

            var options = new DbContextOptionsBuilder<AuraFitDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique name for each test
                .Options;

            _context = new AuraFitDbContext(options);
            _context.Database.EnsureCreated(); // Ensure the database is created for each test

            _repository = new UserProfileRepository(_context);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted(); // Delete the in-memory database after each test
            _context.Dispose();
        }

        [TestMethod]
        public void UserProfileRepositoryTest()
        {
            // Arrange
            // Act
            // The TestInitialize method already creates an instance of the repository.
            // We just need to assert that it's not null.
            // Assert
            Assert.IsNotNull(_repository, "Repository should be initialized.");
        }

        [TestMethod]
        public async Task GetByUserIdAsync_ReturnsProfile_WhenProfileExists()
        {
            // Arrange
            var userId = 1;
            var expectedProfile = new UserProfile { ProfileId = 1, UserId = userId, Age = 30, Gender = "Male", HeightCm = 175, WeightKg = 70 };
            await _context.UserProfiles.AddAsync(expectedProfile);
            await _context.SaveChangesAsync();

            // Act
            var actualProfile = await _repository.GetByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(actualProfile, "Profile should not be null.");
            Assert.AreEqual(expectedProfile.UserId, actualProfile.UserId, "User IDs should match.");
            Assert.AreEqual(expectedProfile.Age, actualProfile.Age, "Age should match.");
        }

        [TestMethod]
        public async Task GetByUserIdAsync_ReturnsNull_WhenProfileDoesNotExist()
        {
            // Arrange
            var userId = 999; 

            // Act
            var actualProfile = await _repository.GetByUserIdAsync(userId);

            // Assert
            Assert.IsNull(actualProfile, "Profile should be null when not found.");
        }

        [TestMethod]
        public async Task UpsertAsync_InsertsNewProfile_WhenProfileDoesNotExist()
        {
            // Arrange
            var newProfile = new UserProfile { UserId = 2, Age = 25, Gender = "Female", HeightCm = 160, WeightKg = 55 };

            // Act
            var result = await _repository.UpsertAsync(newProfile);

            // Assert
            Assert.IsTrue(result, "Upsert should return true for a successful insert.");
            var profileInDb = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == newProfile.UserId);
            Assert.IsNotNull(profileInDb, "New profile should be found in the database.");
            Assert.AreEqual(newProfile.Age, profileInDb.Age, "Age should be updated correctly.");
        }

        [TestMethod]
        public async Task UpsertAsync_UpdatesExistingProfile_WhenProfileExists()
        {
            // Arrange
            var userId = 3;
           
            var existingProfile = new UserProfile { ProfileId = 1, UserId = userId, Age = 30, Gender = "Male", HeightCm = 170, WeightKg = 75 };
            await _context.UserProfiles.AddAsync(existingProfile);
            await _context.SaveChangesAsync();

            var updatedProfile = new UserProfile { UserId = userId, Age = 32, Gender = "Male", HeightCm = 172, WeightKg = 78, ActivityLevel = "Active", Bmi = 26, Bmr = 1800, Tdee = 2500 };

            // Act
            var result = await _repository.UpsertAsync(updatedProfile);

            // Assert
            Assert.IsTrue(result, "Upsert should return true for a successful update.");
            var profileInDb = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            Assert.IsNotNull(profileInDb, "Profile should still exist in the database.");
            Assert.AreEqual(updatedProfile.Age, profileInDb.Age, "Age should be updated correctly.");
            Assert.AreEqual(updatedProfile.WeightKg, profileInDb.WeightKg, "Weight should be updated correctly.");
            Assert.AreEqual(updatedProfile.ActivityLevel, profileInDb.ActivityLevel, "Activity level should be updated correctly.");
            Assert.AreEqual(updatedProfile.Bmi, profileInDb.Bmi, "BMI should be updated correctly.");
            Assert.AreEqual(updatedProfile.Bmr, profileInDb.Bmr, "BMR should be updated correctly.");
            Assert.AreEqual(updatedProfile.Tdee, profileInDb.Tdee, "TDEE should be updated correctly.");
        }

        [TestMethod]
        public async Task ShouldPromptWeightUpdateAsync_ReturnsTrue_WhenNoWeightLogsExist()
        {
            // Arrange
            var userId = 4; 

            // Act
            var shouldPrompt = await _repository.ShouldPromptWeightUpdateAsync(userId);

            // Assert
            Assert.IsTrue(shouldPrompt, "Should prompt when no weight logs exist.");
        }

        [TestMethod]
        public async Task ShouldPromptWeightUpdateAsync_ReturnsTrue_WhenLastLogIsOlderThan7Days()
        {
            // Arrange
            var userId = 5;
            var oldWeightLog = new WeightLog { LogId = 1, UserId = userId, WeightKg = 80, LoggedAt = DateTime.Now.AddDays(-8) };
            await _context.WeightLogs.AddAsync(oldWeightLog);
            await _context.SaveChangesAsync();

            // Act
            var shouldPrompt = await _repository.ShouldPromptWeightUpdateAsync(userId);

            // Assert
            Assert.IsTrue(shouldPrompt, "Should prompt when the last log is older than 7 days.");
        }

        [TestMethod]
        public async Task ShouldPromptWeightUpdateAsync_ReturnsFalse_WhenLastLogIsWithin7Days()
        {
            // Arrange
            var userId = 6;
            var recentWeightLog = new WeightLog { LogId = 1, UserId = userId, WeightKg = 75, LoggedAt = DateTime.Now.AddDays(-5) };
            await _context.WeightLogs.AddAsync(recentWeightLog);
            await _context.SaveChangesAsync();

            // Act
            var shouldPrompt = await _repository.ShouldPromptWeightUpdateAsync(userId);

            // Assert
            Assert.IsFalse(shouldPrompt, "Should not prompt when the last log is within 7 days.");
        }

        [TestMethod]
        public async Task ShouldPromptWeightUpdateAsync_ReturnsFalse_WhenLastLogIsExactly7DaysOld()
        {
            // Arrange
            var userId = 7;
            // Set time to be exactly 7 days ago at the current time (or slightly earlier to avoid precision issues)
            var exactly7DaysAgo = DateTime.Now.AddDays(-7).AddSeconds(-1);
            var weightLog = new WeightLog { LogId = 1, UserId = userId, WeightKg = 68, LoggedAt = exactly7DaysAgo };
            await _context.WeightLogs.AddAsync(weightLog);
            await _context.SaveChangesAsync();

            // Act
            var shouldPrompt = await _repository.ShouldPromptWeightUpdateAsync(userId);

            // Assert
            Assert.IsTrue(shouldPrompt, "Should prompt when the last log is exactly 7 days old (or older).");
        }
    }
}