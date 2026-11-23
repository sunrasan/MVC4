using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class ApplicationUserTests
    {
        [Fact]
        public void ApplicationUser_DefaultConstructor_CreatesInstance()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert
            Assert.NotNull(user);
        }

        [Fact]
        public void ApplicationUser_SetUserName_ReturnsCorrectValue()
        {
            // Arrange
            var user = new ApplicationUser();

            // Act
            user.UserName = "testuser";

            // Assert
            Assert.Equal("testuser", user.UserName);
        }

        [Fact]
        public void ApplicationUser_SetEmail_ReturnsCorrectValue()
        {
            // Arrange
            var user = new ApplicationUser();

            // Act
            user.Email = "test@example.com";

            // Assert
            Assert.Equal("test@example.com", user.Email);
        }

        [Fact]
        public void ApplicationUser_SetId_ReturnsCorrectValue()
        {
            // Arrange
            var user = new ApplicationUser();
            var id = Guid.NewGuid().ToString();

            // Act
            user.Id = id;

            // Assert
            Assert.Equal(id, user.Id);
        }

        [Fact]
        public void ApplicationUser_IsInstanceOfIdentityUser_True()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert
            Assert.IsAssignableFrom<Microsoft.AspNetCore.Identity.IdentityUser>(user);
        }

        [Fact]
        public void ApplicationUser_SetEmailConfirmed_ReturnsCorrectValue()
        {
            // Arrange
            var user = new ApplicationUser();

            // Act
            user.EmailConfirmed = true;

            // Assert
            Assert.True(user.EmailConfirmed);
        }

        [Fact]
        public void ApplicationUser_InitializeWithValues_AllPropertiesSet()
        {
            // Arrange & Act
            var user = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@musicstore.local",
                EmailConfirmed = true
            };

            // Assert
            Assert.Equal("admin", user.UserName);
            Assert.Equal("admin@musicstore.local", user.Email);
            Assert.True(user.EmailConfirmed);
        }
    }
}
