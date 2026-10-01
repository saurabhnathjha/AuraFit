using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AuraFitWebService.Controllers;
using AuraFitDataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace AuraFitAPITestin
{
    public class ExerciseCatalogControllerTest
    {
        private readonly ExerciseCatalogController _controller;
        private readonly AuraFitDbContext _context;

        public ExerciseCatalogControllerTest()
        {
            // Setup in-memory EF database
            var options = new DbContextOptionsBuilder<AuraFitDbContext>()
                .UseInMemoryDatabase(databaseName: "ExerciseCatalogTestDb")
                .Options;

            _context = new AuraFitDbContext(options);

            // Seed test data if not already seeded
            if (!_context.ExerciseCatalog.Any())
            {
                _context.ExerciseCatalog.AddRange(
                    new ExerciseCatalog { ExerciseCatalogId = 1, Name = "Push-Up" },
                    new ExerciseCatalog { ExerciseCatalogId = 2, Name = "Squat" },
                    new ExerciseCatalog { ExerciseCatalogId = 3, Name = "Lunge" }
                );
                _context.SaveChanges();
            }

            var loggerMock = new Mock<ILogger<ExerciseCatalogController>>();
            _controller = new ExerciseCatalogController(_context, loggerMock.Object);
        }

        [Fact]
        public async Task GetAllExercises_ReturnsOkWithExerciseList()
        {
            // Act
            var result = await _controller.GetAllExercises();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var exercises = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

            Assert.Equal(3, exercises.Count()); // Check that we received 3 exercises
        }

        [Fact]
        public async Task GetAllExercises_ReturnsBadRequest_OnException()
        {
            // Arrange - simulate a broken context
            var mockContext = new Mock<AuraFitDbContext>();
            var loggerMock = new Mock<ILogger<ExerciseCatalogController>>();

            var brokenController = new ExerciseCatalogController(mockContext.Object, loggerMock.Object);

            mockContext.Setup(c => c.ExerciseCatalog).Throws(new Exception("DB Error"));

            // Act
            var result = await brokenController.GetAllExercises();

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("An error occurred while fetching exercises.", badRequest.Value);
        }
    }
}
