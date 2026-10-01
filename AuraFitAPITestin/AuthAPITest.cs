using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

using AuraFitWebService.Controllers;
using AuraFitDataAccessLayer.Repositories.Interfaces;
using AuraFitWebService.Utilities;
using AuraFitDataAccessLayer.Models;
using AuraFitWebService.DTOs;

namespace AuraFitAPITestin
{
    public class AuthAPITest
    {
        private readonly AuthController _controller;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly Mock<ITokenService> _mockTokenService;

        public AuthAPITest()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _mockTokenService = new Mock<ITokenService>();

            _controller = new AuthController(
                _mockUserRepo.Object,
                _mockTokenService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserIsNew()
        {
            // Arrange
            var dto = new UserRegisterDTO
            {
                Username = "newuser",
                Password = "securepassword"
            };

            _mockUserRepo.Setup(r => r.GetUserByUsernameAsync(dto.Username))
                         .ReturnsAsync((User)null);

            _mockUserRepo.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic parsed = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.Equal("User registered successfully", (string)parsed.message);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUsernameExists()
        {
            // Arrange
            var dto = new UserRegisterDTO
            {
                Username = "existinguser",
                Password = "any"
            };

            _mockUserRepo.Setup(r => r.GetUserByUsernameAsync(dto.Username))
                         .ReturnsAsync(new User { Username = dto.Username });

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already exists", badRequest.Value);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var dto = new UserRegisterDTO
            {
                Username = "erroruser",
                Password = "pass"
            };

            _mockUserRepo.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>()))
                         .ThrowsAsync(new Exception("DB down"));

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("An error occurred during registration.", badRequest.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new UserLoginDTO
            {
                Username = "validuser",
                Password = "correctpassword"
            };

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                UserId = 1
            };

            _mockUserRepo.Setup(r => r.GetUserByUsernameAsync(dto.Username))
                         .ReturnsAsync(user);

            _mockTokenService.Setup(t => t.CreateToken(user))
                             .Returns("mocked-jwt-token");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            dynamic parsed = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.Equal("mocked-jwt-token", (string)parsed.token);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new UserLoginDTO
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            var fakeUser = new User
            {
                Username = "testuser",
                PasswordHash = PasswordHasher.Hash("correctpassword")
            };


            _mockUserRepo.Setup(repo => repo.GetUserByUsernameAsync(dto.Username))
                         .ReturnsAsync(fakeUser);

            var controller = new AuthController(
            _mockUserRepo.Object,
            _mockTokenService.Object,
            _mockLogger.Object
            );
 

            // Act
            var result = await controller.Login(dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

    }
}
