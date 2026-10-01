using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AuraFitWebService.Controllers;
using AuraFitDataAccessLayer.Models;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AuraFitWebService.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace AuraFitAPITestin
{
    public class WorkoutLogControllerTests
    {
        private readonly Mock<AuraFitDbContext> _mockDbContext;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IWorkoutLogRepository> _mockWorkoutRepo;
        private readonly Mock<ILogger<WorkoutLogController>> _mockLogger;
        private readonly WorkoutLogController _controller;
        private readonly int _testUserId = 101;

        public WorkoutLogControllerTests()
        {
            // Setup mocks
            _mockDbContext = new Mock<AuraFitDbContext>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockWorkoutRepo = new Mock<IWorkoutLogRepository>();
            _mockLogger = new Mock<ILogger<WorkoutLogController>>();

            // Simulate authenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
            }, "mock"));

            var httpContext = new DefaultHttpContext() { User = user };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Setup controller
            _controller = new WorkoutLogController(
                _mockDbContext.Object,
                _mockHttpContextAccessor.Object,
                _mockWorkoutRepo.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetWorkoutSummary_ReturnsOk()
        {
            var date = DateTime.Today;
            _mockWorkoutRepo.Setup(repo => repo.GetTotalCaloriesBurnedForUserOnDateAsync(_testUserId, date))
                .ReturnsAsync(400);

            var result = await _controller.GetWorkoutSummary(date) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            dynamic response = result.Value;
            Assert.Equal(400, (int)response.totalCaloriesBurned);
        }

        [Fact]
        public async Task GetWorkoutsInRange_ReturnsWorkouts()
        {
            var workouts = new List<WorkoutLog>
            {
                new WorkoutLog
                {
                    WorkoutLogId = 1,
                    UserId = _testUserId,
                    LoggedAt = DateTime.Today,
                    DurationMin = 45,
                    CaloriesBurned = 250,
                    WorkoutLogExercises = new List<WorkoutLogExercise>
                    {
                        new WorkoutLogExercise
                        {
                            ExerciseCatalog = new ExerciseCatalog { Name = "Pushup", Category = "Strength" },
                            Reps = 10,
                            Sets = 3,
                            DurationMin = 15,
                            CaloriesBurned = 100
                        }
                    },
                    WorkoutTemplate = new WorkoutTemplate { Name = "Morning Routine" }
                }
            };

            var mockDbSet = CreateMockDbSet(workouts.AsQueryable());
            _mockDbContext.Setup(x => x.WorkoutLogs).Returns(mockDbSet.Object);

            var result = await _controller.GetWorkoutsInRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var list = result.Value as IEnumerable<WorkoutLogReadDTO>;
            Assert.Single(list);
        }

        [Fact]
        public async Task DeleteWorkout_ReturnsNoContent_WhenExists()
        {
            var workout = new WorkoutLog { WorkoutLogId = 1, UserId = _testUserId };
            var dbSet = new List<WorkoutLog> { workout }.AsQueryable();

            var mockSet = CreateMockDbSet(dbSet);
            _mockDbContext.Setup(x => x.WorkoutLogs).Returns(mockSet.Object);
            mockSet.Setup(m => m.Remove(It.IsAny<WorkoutLog>()));
            _mockDbContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _controller.DeleteWorkout(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetWorkout_ReturnsWorkout_WhenFound()
        {
            var workout = new WorkoutLog
            {
                WorkoutLogId = 1,
                UserId = _testUserId,
                WorkoutLogExercises = new List<WorkoutLogExercise>()
            };

            var dbSet = CreateMockDbSet(new List<WorkoutLog> { workout }.AsQueryable());
            _mockDbContext.Setup(x => x.WorkoutLogs).Returns(dbSet.Object);

            var result = await _controller.GetWorkout(1) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task LogWorkout_ReturnsOkResult()
        {
            var dto = new WorkoutLogCreateDTO
            {
                WorkoutTemplateId = 1,
                LoggedAt = DateTime.Now,
                Exercises = new List<WorkoutLogExerciseCreateDTO>
                {
                    new WorkoutLogExerciseCreateDTO
                    {
                        ExerciseCatalogId = 1,
                        Reps = 10,
                        Sets = 3,
                        DurationMin = 15
                    }
                }
            };

            _mockDbContext.Setup(x => x.UserProfiles)
                .Returns(CreateMockDbSet(new List<UserProfile> { new UserProfile { UserId = _testUserId, WeightKg = 70 } }.AsQueryable()).Object);

            _mockDbContext.Setup(x => x.ExerciseCatalog)
                .Returns(CreateMockDbSet(new List<ExerciseCatalog> { new ExerciseCatalog { ExerciseCatalogId = 1, METValue = 5.0 } }.AsQueryable()).Object);

            _mockDbContext.Setup(x => x.WorkoutLogs.Add(It.IsAny<WorkoutLog>()));
            _mockDbContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _controller.LogWorkout(dto) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
    }
}