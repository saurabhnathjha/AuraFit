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
    public class UserRepositoryTests
    {
        private AuraFitDbContext _context;
        private UserRepository _userRepository;

        // This method runs before each test method
        [TestInitialize]
        public void TestInitialize()
        {
            // Configure in-memory database options
            var options = new DbContextOptionsBuilder<AuraFitDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test run
                .Options;

            _context = new AuraFitDbContext(options);
            _userRepository = new UserRepository(_context);

            // Seed some data for tests
            // Removed explicit Id assignments to let EF Core's in-memory provider handle ID generation
            _context.Users.Add(new User { Username = "testuser1", Email = "test1@example.com", PasswordHash = "hash1" });
            _context.Users.Add(new User { Username = "testuser2", Email = "test2@example.com", PasswordHash = "hash2" });
            _context.SaveChanges();
        }

        // This method runs after each test method
        [TestCleanup]
        public void TestCleanup()
        {
            _context.Database.EnsureDeleted(); // Ensure the database is deleted after each test
            _context.Dispose();
        }

        [TestMethod()]
        public void UserRepositoryTest()
        {
            // This test method can be used to ensure the constructor initializes correctly.
            // Since TestInitialize already sets up the repository, we can just assert that it's not null.
            Assert.IsNotNull(_userRepository, "UserRepository should be initialized.");
            Assert.IsNotNull(_context, "DbContext should be initialized.");
        }

        [TestMethod()]
        public async Task GetUserByUsernameAsyncTest_UserExists()
        {
            // Arrange
            string username = "testuser1";

            // Act
            User user = await _userRepository.GetUserByUsernameAsync(username);

            // Assert
            Assert.IsNotNull(user, "User should not be null when found.");
            Assert.AreEqual(username, user.Username, "The retrieved user's username should match the requested username.");
            Assert.IsTrue(user.UserId > 0, "The retrieved user should have a valid ID (greater than 0)."); // Adjusted assertion for auto-generated ID
        }

        [TestMethod()]
        public async Task GetUserByUsernameAsyncTest_UserDoesNotExist()
        {
            // Arrange
            string username = "nonexistentuser";

            // Act
            User user = await _userRepository.GetUserByUsernameAsync(username);

            // Assert
            Assert.IsNull(user, "User should be null when not found.");
        }

        


        [TestMethod()]
        public async Task AddUserAsyncTest_Success()
        {
            // Arrange
            // Removed explicit Id assignment to let EF Core's in-memory provider handle ID generation
            var newUser = new User { Username = "newuser", Email = "new@example.com", PasswordHash = "newhash" };

            // Act
            await _userRepository.AddUserAsync(newUser);

            // Assert
            // Verify that the user is now in the database
            var addedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
            Assert.IsNotNull(addedUser, "The new user should be added to the database.");
            Assert.AreEqual(newUser.Username, addedUser.Username, "Added user's username should match.");
            Assert.IsTrue(addedUser.UserId > 0, "Added user should have a valid ID."); // Adjusted assertion
            Assert.AreEqual(3, await _context.Users.CountAsync(), "There should now be 3 users in the database.");
        }
        [TestMethod()]
        public async Task AddUserAsyncTest_DuplicateUsernameThrowsException() // Renamed test method
        {
            // Arrange
            var duplicateUsername = "testuser1";
            var duplicateUser = new User { Username = duplicateUsername, Email = "duplicate@example.com", PasswordHash = "duplicatehash" };

            // DIAGNOSTIC: Verify the initial user exists directly in the context BEFORE calling AddUserAsync
            var initialExistingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == duplicateUsername);
            Assert.IsNotNull(initialExistingUser, $"Diagnostic: '{duplicateUsername}' should exist in the context before attempting to add duplicate. If this fails, TestInitialize setup is incorrect.");

            // Get initial count of users to ensure it doesn't change
            var initialUserCount = await _context.Users.CountAsync();

            // Act & Assert
            // Expect an InvalidOperationException due to duplicate username as per the updated repository logic
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await _userRepository.AddUserAsync(duplicateUser);
            }, $"Expected an InvalidOperationException when trying to add a user with duplicate username '{duplicateUser.Username}'.");

            // Assert that the count of users has not increased
            var currentUserCount = await _context.Users.CountAsync();
            Assert.AreEqual(initialUserCount, currentUserCount, "The number of users should not increase after attempting to add a duplicate username.");

            // Optionally, verify that the 'duplicate' user instance wasn't actually saved as a new entry.
            var foundUser = await _context.Users.Where(u => u.Username == duplicateUsername).ToListAsync();
            Assert.AreEqual(1, foundUser.Count, $"Only one user with '{duplicateUsername}' username should exist in the database after a failed addition attempt.");
        }
    }
}