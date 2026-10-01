using AuraFitDataAccessLayer.Models;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using AuraFitWebService.Controllers;
using AuraFitWebService.DTOs;
using AuraFitWebService.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Org.BouncyCastle.Security;
using System.Security.Claims;

using Newtonsoft.Json.Linq;

namespace AuraFitAPITestin
{
    public class UserProfileTest
    {
        private readonly Mock<IUserProfileRepository> _mockRepo;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ILogger<UserProfileController>> _mockLogger;
        private readonly UserProfileController _controller;

        public UserProfileTest()
        {
            _mockRepo = new Mock<IUserProfileRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<UserProfileController>>();

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, "123")
            }, "mock"))
            };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _controller = new UserProfileController(
                _mockRepo.Object,
                _mockHttpContextAccessor.Object,
                _mockLogger.Object
            );
        }
        [Fact]
        public async Task GetMyProfile_ReturnsOk_WhenProfileExists()
        {
            // Arrange
            var profile = new UserProfile
            {
                UserId = 123,
                Age = 30,
                Gender = "Male",
                HeightCm = 180,
                WeightKg = 80
            };

            _mockRepo.Setup(r => r.GetByUserIdAsync(123)).ReturnsAsync(profile);

            // Act
            var result = await _controller.GetMyProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserProfile>(okResult.Value);
            Assert.Equal(123, returnValue.UserId);
        }
        [Fact]
        public async Task GetMyProfile_ReturnsNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByUserIdAsync(123)).ReturnsAsync((UserProfile)null);

            // Act
            var result = await _controller.GetMyProfile();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetMyProfile_ReturnsUnauthorized_WhenClaimIsMissing()
        {
            // Arrange
            var contextWithoutClaim = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // no claims
            };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(contextWithoutClaim);

            var controller = new UserProfileController(
                _mockRepo.Object,
                _mockHttpContextAccessor.Object,
                _mockLogger.Object
            );

            // Act
            var result = await controller.GetMyProfile();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
        [Fact]
        public async Task GetMyProfile_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<int>())).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetMyProfile();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }



        [Fact]
        public async Task CreateOrUpdateProfile_ReturnsOk_WhenUpsertSucceeds()
        {
            // Arrange
            var dto = new UserProfileDTO
            {
                Age = 30,
                Gender = "Male",
                HeightCm = 180,
                WeightKg = 75,
                ActivityLevel = "moderately active"
            };

            _mockRepo.Setup(r => r.UpsertAsync(It.IsAny<UserProfile>())).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateOrUpdateProfile(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<UserProfile>(okResult.Value);
            Assert.Equal(123, profile.UserId); // 123 is from the test's mocked ClaimsPrincipal
            Assert.Equal(dto.Age, profile.Age);
            Assert.Equal(dto.HeightCm, profile.HeightCm);
            Assert.Equal(dto.WeightKg, profile.WeightKg);
            Assert.Equal(dto.Gender, profile.Gender);
            Assert.Equal(dto.ActivityLevel, profile.ActivityLevel);
        }

        [Fact]
        public async Task CreateOrUpdateProfile_ReturnsServerError_WhenUpsertFails()
        {
            // Arrange
            var dto = new UserProfileDTO
            {
                Age = 35,
                Gender = "Other",
                HeightCm = 170,
                WeightKg = 70,
                ActivityLevel = "lightly active" 
            };

            _mockRepo.Setup(r => r.UpsertAsync(It.IsAny<UserProfile>())).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateOrUpdateProfile(dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

     
        [Theory]
        [InlineData("sedentary", 1.2)]
        [InlineData("lightly active", 1.375)]
        [InlineData("moderately active", 1.55)]
        [InlineData("very active", 1.725)]
        [InlineData("extra active", 1.9)]
        public async Task CreateOrUpdateProfile_ValidActivityLevels_ShouldCalculateCorrectTDEE(string activityLevel, double expectedFactor)
        {
            // Arrange
            var dto = new UserProfileDTO
            {
                Age = 30,
                Gender = "Male",
                HeightCm = 180,
                WeightKg = 75,
                ActivityLevel = activityLevel
            };

            // Calculate expected BMR and TDEE
            var bmr = FitnessCalculator.CalculateBMR(dto.WeightKg, dto.HeightCm, dto.Age, dto.Gender);
            var expectedTDEE = Math.Round(bmr * expectedFactor, 2);

            UserProfile capturedProfile = null;
            _mockRepo.Setup(r => r.UpsertAsync(It.IsAny<UserProfile>()))
                     .Callback<UserProfile>(p => capturedProfile = p)
                     .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateOrUpdateProfile(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<UserProfile>(okResult.Value);

            Assert.NotNull(capturedProfile);
            Assert.Equal(expectedTDEE, Math.Round(capturedProfile.Tdee.Value, 2));
        }


        [Fact]
        public async Task ShouldUpdateWeight_ReturnsOkWithTrue_WhenRepoReturnsTrue()
        {
            // Arrange
            int userId = 123;
            _mockRepo.Setup(r => r.ShouldPromptWeightUpdateAsync(userId))
                     .ReturnsAsync(true);

            // Act
            var result = await _controller.ShouldUpdateWeight();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Dictionary<string, object>>(okResult.Value
                .GetType().GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(okResult.Value)));

            Assert.True((bool)response["shouldUpdate"]);
        }
        [Fact]
        public async Task ShouldUpdateWeight_ReturnsOkWithFalse_WhenRepoReturnsFalse()
        {
            // Arrange
            int userId = 123;
            bool expected = false;

            _mockRepo.Setup(r => r.ShouldPromptWeightUpdateAsync(userId))
                     .ReturnsAsync(expected);

            // Act
            var result = await _controller.ShouldUpdateWeight();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JObject.FromObject(okResult.Value);

            Assert.Equal(expected, json["shouldUpdate"]?.Value<bool>());
        }
        [Fact]
        public async Task ShouldUpdateWeight_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            int userId = 123;
            _mockRepo.Setup(r => r.ShouldPromptWeightUpdateAsync(userId))
                     .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.ShouldUpdateWeight();

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Something went wrong", badRequest.Value.ToString());
        }


        [Fact]
        public async Task ShouldUpdateWeight_ReturnsOkWithExpectedValue()
        {
            // Arrange
            int userId = 123;
            bool expected = true;

            _mockRepo.Setup(r => r.ShouldPromptWeightUpdateAsync(userId))
                     .ReturnsAsync(expected);

            // Act
            var result = await _controller.ShouldUpdateWeight();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JObject.FromObject(okResult.Value);

            Assert.Equal(expected, json["shouldUpdate"].Value<bool>());
        }


        [Fact]
        public async Task ShouldUpdateWeight_ReturnsBadRequest_OnException()
        {
            // Arrange
            int userId = 123;
            _mockRepo.Setup(r => r.ShouldPromptWeightUpdateAsync(userId))
                     .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _controller.ShouldUpdateWeight();

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Database failure", badRequest.Value.ToString());
        }






    }
}