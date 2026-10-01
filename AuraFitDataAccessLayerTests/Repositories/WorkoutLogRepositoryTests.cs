#region Testing
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
//    public class WorkoutLogRepositoryTests
//    {
//        [TestMethod()]
//        public void WorkoutLogRepositoryTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void GetAllLogsByUserAsyncTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void GetLogByIdAsyncTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void GetLogByIdAndUserAsyncTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void AddWorkoutLogAsyncTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void AddExerciseToLogAsyncTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void UpdateWorkoutLogAsyncTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void GetTotalCaloriesBurnedForUserOnDateAsyncTest()
//        {
//            Assert.Fail();
//        }

//        [TestMethod()]
//        public void DeleteWorkoutLogAsyncTest()
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
    public class WorkoutLogRepositoryTests
    {
        private AuraFitDbContext _context;
        private WorkoutLogRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuraFitDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
                .Options;

            _context = new AuraFitDbContext(options);
            _repository = new WorkoutLogRepository(_context);
        }
        [TestMethod]
        public async Task GetAllLogsByUserAsync_ReturnsLogs_WhenLogsExist()
        {
            var log1 = new WorkoutLog
            {
                UserId = 1,
                LoggedAt = DateTime.UtcNow.AddDays(-1),
                CaloriesBurned = 200
            };
            var log2 = new WorkoutLog
            {
                UserId = 1,
                LoggedAt = DateTime.UtcNow,
                CaloriesBurned = 300
            };

            await _context.WorkoutLogs.AddRangeAsync(log1, log2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllLogsByUserAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(300, result.First().CaloriesBurned); // Ordered descending
        }
        [TestMethod]
        public async Task GetAllLogsByUserAsync_ReturnsEmpty_WhenNoLogsExist()
        {
            var result = await _repository.GetAllLogsByUserAsync(999); // no data for this user
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetAllLogsByUserAsync_ReturnsNull_WhenExceptionOccurs()
        {
            _context.Dispose(); // force exception
            var brokenRepo = new WorkoutLogRepository(_context);

            var result = await brokenRepo.GetAllLogsByUserAsync(1);

            Assert.IsNull(result);
        }



        [TestMethod]
        public async Task GetLogByIdAsync_LogExists_ReturnsLogWithExercises()
        {
            // Arrange
            var log = new WorkoutLog { WorkoutLogId = 1, UserId = 1 };
            var exercise = new WorkoutLogExercise { WorkoutLogExerciseId = 1, WorkoutLogId = 1 };

            _context.WorkoutLogs.Add(log);
            _context.WorkoutLogExercises.Add(exercise);
            await _context.SaveChangesAsync();

            // var repository = new WorkoutLogRepository(_context);

            // Act
            var result = await _repository.GetLogByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.WorkoutLogId);
            Assert.IsNotNull(result.WorkoutLogExercises);
            Assert.AreEqual(1, result.WorkoutLogExercises.Count);
        }

        [TestMethod]
        public async Task GetLogByIdAsync_LogDoesNotExist_ReturnsNull()
        {
            // Arrange
            //var repository = new WorkoutLogRepository(_context);

            // Act
            var result = await _repository.GetLogByIdAsync(999); // Not inserted

            // Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task GetLogByIdAndUserAsync_ReturnsCorrectLog()
        {
            var log = new WorkoutLog
            {
                WorkoutLogId = 1,
                UserId = 42,
                CaloriesBurned = 500
            };

            _context.WorkoutLogs.Add(log);
            await _context.SaveChangesAsync();

            //var repo = new WorkoutLogRepository(context);

            // Act
            var result = await _repository.GetLogByIdAndUserAsync(1, 42);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(42, result.UserId);
            Assert.AreEqual(1, result.WorkoutLogId);
        }


        [TestMethod]
        public async Task GetLogByIdAndUserAsync_ReturnsNull_WhenUserIdDoesNotMatch()
        {
            // Arrange

            var log = new WorkoutLog { WorkoutLogId = 2, UserId = 123 };
            _context.WorkoutLogs.Add(log);
            await _context.SaveChangesAsync();



            // Act
            var result = await _repository.GetLogByIdAndUserAsync(2, 999); // Wrong userId

            // Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public async Task GetLogByIdAndUserAsync_ReturnsNull_WhenLogDoesNotExist()
        {

            // Act
            var result = await _repository.GetLogByIdAndUserAsync(99, 1); // No such log

            // Assert
            Assert.IsNull(result);
        }







        [TestMethod]
        public async Task AddWorkoutLogAsync_ValidLog_ReturnsLogWithId()
        {
            // Arrange


            var user = new User { Username = "testuser", PasswordHash = "hashed" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var log = new WorkoutLog
            {
                UserId = user.UserId,
                CaloriesBurned = 200,
                DurationMin = 45
            };



            // Act
            var result = await _repository.AddWorkoutLogAsync(log);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.WorkoutLogId > 0);
            Assert.AreEqual(200, result.CaloriesBurned);
        }

        [TestMethod]
        public async Task AddWorkoutLogAsync_InvalidUserId_ReturnsNull()
        {
            // Arrange

            var log = new WorkoutLog
            {
                UserId = 9999, // doesn't exist
                CaloriesBurned = 150
            };


            // Act
            var result = await _repository.AddWorkoutLogAsync(log);

            // Assert
            Assert.IsNull(result);
        }




        [TestMethod]
        public async Task AddExerciseToLogAsync_ValidExercise_AddsSuccessfully()
        {
            // Seed user and workout log
            var user = new User { Username = "test", PasswordHash = "hash" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var log = new WorkoutLog
            {
                UserId = user.UserId,
                LoggedAt = DateTime.UtcNow
            };
            _context.WorkoutLogs.Add(log);
            await _context.SaveChangesAsync();
            var exercise = new WorkoutLogExercise
            {
                WorkoutLogId = log.WorkoutLogId,
                ExerciseCatalogId = 1,  // You need to seed this too if FK is enforced
                Reps = 10,
                Sets = 3,
                WeightKg = 50
            };
            // Act
            await _repository.AddExerciseToLogAsync(exercise);

            // Assert
            Assert.AreEqual(1, _context.WorkoutLogExercises.Count());

        }






        [TestMethod]
        public async Task UpdateWorkoutLogAsync_ValidUpdate_UpdatesLogAndExercises()
        {


            var user = new User { Username = "user1", PasswordHash = "hash" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var log = new WorkoutLog
            {
                UserId = user.UserId,
                DurationMin = 30,
                CaloriesBurned = 200,
                LoggedAt = DateTime.UtcNow
            };
            _context.WorkoutLogs.Add(log);
            await _context.SaveChangesAsync();

            var originalExercise = new WorkoutLogExercise
            {
                WorkoutLogId = log.WorkoutLogId,
                ExerciseCatalogId = 1,
                Reps = 10,
                Sets = 3,
                WeightKg = 50
            };
            _context.WorkoutLogExercises.Add(originalExercise);
            await _context.SaveChangesAsync();



            var updatedExercises = new List<WorkoutLogExercise>
            {
              new WorkoutLogExercise
             {
                WorkoutLogId = log.WorkoutLogId,
                ExerciseCatalogId = 2,
                 Reps = 15,
                 Sets = 4,
                WeightKg = 60
              }
             };

            // Act
            await _repository.UpdateWorkoutLogAsync(log, updatedExercises);

            // Assert
            var updated = await _context.WorkoutLogExercises
                .Where(e => e.WorkoutLogId == log.WorkoutLogId)
                .ToListAsync();

            Assert.AreEqual(1, updated.Count);
            Assert.AreEqual(2, updated[0].ExerciseCatalogId); // new exercise catalog ID
            Assert.AreEqual(15, updated[0].Reps);
        }



        [TestMethod]
        public async Task GetTotalCaloriesBurnedForUserOnDateAsync_WithValidData_ReturnsCorrectSum()
        {


            var date = DateTime.Today;


            _context.WorkoutLogs.AddRange(
                new WorkoutLog { UserId = 1, CaloriesBurned = 200, LoggedAt = date },
                new WorkoutLog { UserId = 1, CaloriesBurned = 300, LoggedAt = date },
                new WorkoutLog { UserId = 1, CaloriesBurned = 100, LoggedAt = date.AddDays(-1) }, // different date
                new WorkoutLog { UserId = 2, CaloriesBurned = 500, LoggedAt = date } // different user
            );
            await _context.SaveChangesAsync();




            // Act
            var result = await _repository.GetTotalCaloriesBurnedForUserOnDateAsync(1, date);

            // Assert
            Assert.AreEqual(500, result); // Only 200 + 300 for user 1 on the correct date
        }

        [TestMethod]
        public async Task GetTotalCaloriesBurnedForUserOnDateAsync_WithNoMatchingLogs_ReturnsZero()
        {


            var date = DateTime.Today;




            // Act
            var result = await _repository.GetTotalCaloriesBurnedForUserOnDateAsync(1, date);

            // Assert
            Assert.AreEqual(0, result);
        }
        [TestMethod]
        public async Task DeleteWorkoutLogAsync_WithValidLog_RemovesLogFromDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AuraFitDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteWorkoutLogDb")
                .Options;

            var log = new WorkoutLog
            {
                WorkoutLogId = 1,
                UserId = 1,
                DurationMin = 45,
                CaloriesBurned = 300,
                LoggedAt = DateTime.Now
            };

            using (var context = new AuraFitDbContext(options))
            {
                context.WorkoutLogs.Add(log);
                await context.SaveChangesAsync();
            }

            using (var context = new AuraFitDbContext(options))
            {
                var repo = new WorkoutLogRepository(context);

                // Act
                await repo.DeleteWorkoutLogAsync(log);
            }

            using (var context = new AuraFitDbContext(options))
            {
                // Assert
                var deleted = await context.WorkoutLogs.FindAsync(1);
                Assert.IsNull(deleted); // The log should be deleted
            }
        }

    }




}