using Xunit;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuraFitWebService.Controllers;
using AuraFitWebService.Services;
using AuraFitDataAccessLayer.Models;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using AuraFitWebService.DTOs;

public class MealsControllerTest
{
    private readonly Mock<IMealRepository> _mockMealRepo;
    private readonly Mock<NutritionixService> _mockNutritionixService;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<ILogger<MealsController>> _loggerMock;
    private readonly MealsController _controller;

    public MealsControllerTest()
    {
        _mockMealRepo = new Mock<IMealRepository>();
        _mockNutritionixService = new Mock<NutritionixService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _loggerMock = new Mock<ILogger<MealsController>>();

        _controller = new MealsController(
            _mockMealRepo.Object,
            _mockNutritionixService.Object,
            _httpContextAccessorMock.Object,
            _loggerMock.Object
        );

        SetFakeUserInContext(); // Ensures controller gets userId from claims
    }

    private void SetFakeUserInContext()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123")
        }, "mock"));

        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }

    [Fact]
    public async Task LogMeal_ReturnsOk_WithCorrectMealData()
    {
        // Arrange
        var dto = new MealLogRequestDTO
        {
            Name = "Lunch",
            Description = "Healthy meal",
            Query = "2 eggs and 1 toast"
        };

        var parsedItems = new List<ParsedFoodItem>
        {
            new ParsedFoodItem { Name = "Egg", Quantity = "2", Calories = 150 },
            new ParsedFoodItem { Name = "Toast", Quantity = "1", Calories = 100 }
        };

        _mockNutritionixService.Setup(x => x.ParseMealQueryAsync(dto.Query))
            .ReturnsAsync(parsedItems);

        _mockMealRepo.Setup(x => x.CreateMealAsync(It.IsAny<Meal>())).ReturnsAsync(1);

        // Act
        var result = await _controller.LogMeal(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMeal = Assert.IsType<Meal>(okResult.Value);
        Assert.Equal(dto.Name, returnedMeal.Name);
        Assert.Equal(250, returnedMeal.TotalCalories); // 150 + 100
        Assert.Equal(2, returnedMeal.FoodItems.Count);
    }
}