using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class AccountModelsTests
    {
        // ---- RegisterExternalLoginModel ----

        [Fact]
        public void RegisterExternalLoginModel_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var model = new RegisterExternalLoginModel();

            // Assert
            Assert.Equal(string.Empty, model.UserName);
            Assert.Equal(string.Empty, model.ExternalLoginData);
        }

        [Fact]
        public void RegisterExternalLoginModel_SetUserName_ReturnsCorrectValue()
        {
            // Arrange
            var model = new RegisterExternalLoginModel();

            // Act
            model.UserName = "testuser";

            // Assert
            Assert.Equal("testuser", model.UserName);
        }

        [Fact]
        public void RegisterExternalLoginModel_SetExternalLoginData_ReturnsCorrectValue()
        {
            // Arrange
            var model = new RegisterExternalLoginModel();

            // Act
            model.ExternalLoginData = "some-token-data";

            // Assert
            Assert.Equal("some-token-data", model.ExternalLoginData);
        }

        // ---- LocalPasswordModel ----

        [Fact]
        public void LocalPasswordModel_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var model = new LocalPasswordModel();

            // Assert
            Assert.Equal(string.Empty, model.OldPassword);
            Assert.Equal(string.Empty, model.NewPassword);
            Assert.Equal(string.Empty, model.ConfirmPassword);
        }

        [Fact]
        public void LocalPasswordModel_SetOldPassword_ReturnsCorrectValue()
        {
            // Arrange
            var model = new LocalPasswordModel();

            // Act
            model.OldPassword = "OldPass123";

            // Assert
            Assert.Equal("OldPass123", model.OldPassword);
        }

        [Fact]
        public void LocalPasswordModel_SetNewPassword_ReturnsCorrectValue()
        {
            // Arrange
            var model = new LocalPasswordModel();

            // Act
            model.NewPassword = "NewPass456";

            // Assert
            Assert.Equal("NewPass456", model.NewPassword);
        }

        [Fact]
        public void LocalPasswordModel_SetConfirmPassword_ReturnsCorrectValue()
        {
            // Arrange
            var model = new LocalPasswordModel();

            // Act
            model.ConfirmPassword = "NewPass456";

            // Assert
            Assert.Equal("NewPass456", model.ConfirmPassword);
        }

        // ---- LoginModel ----

        [Fact]
        public void LoginModel_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var model = new LoginModel();

            // Assert
            Assert.Equal(string.Empty, model.UserName);
            Assert.Equal(string.Empty, model.Password);
            Assert.False(model.RememberMe);
        }

        [Fact]
        public void LoginModel_SetUserName_ReturnsCorrectValue()
        {
            // Arrange
            var model = new LoginModel();

            // Act
            model.UserName = "admin";

            // Assert
            Assert.Equal("admin", model.UserName);
        }

        [Fact]
        public void LoginModel_SetPassword_ReturnsCorrectValue()
        {
            // Arrange
            var model = new LoginModel();

            // Act
            model.Password = "secret";

            // Assert
            Assert.Equal("secret", model.Password);
        }

        [Fact]
        public void LoginModel_SetRememberMe_ReturnsCorrectValue()
        {
            // Arrange
            var model = new LoginModel();

            // Act
            model.RememberMe = true;

            // Assert
            Assert.True(model.RememberMe);
        }

        // ---- RegisterModel ----

        [Fact]
        public void RegisterModel_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var model = new RegisterModel();

            // Assert
            Assert.Equal(string.Empty, model.UserName);
            Assert.Equal(string.Empty, model.Password);
            Assert.Equal(string.Empty, model.ConfirmPassword);
        }

        [Fact]
        public void RegisterModel_SetUserName_ReturnsCorrectValue()
        {
            // Arrange
            var model = new RegisterModel();

            // Act
            model.UserName = "newuser";

            // Assert
            Assert.Equal("newuser", model.UserName);
        }

        [Fact]
        public void RegisterModel_SetPassword_ReturnsCorrectValue()
        {
            // Arrange
            var model = new RegisterModel();

            // Act
            model.Password = "Password123";

            // Assert
            Assert.Equal("Password123", model.Password);
        }

        [Fact]
        public void RegisterModel_SetConfirmPassword_ReturnsCorrectValue()
        {
            // Arrange
            var model = new RegisterModel();

            // Act
            model.ConfirmPassword = "Password123";

            // Assert
            Assert.Equal("Password123", model.ConfirmPassword);
        }

        // ---- ExternalLogin ----

        [Fact]
        public void ExternalLogin_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var login = new ExternalLogin();

            // Assert
            Assert.Equal(string.Empty, login.Provider);
            Assert.Equal(string.Empty, login.ProviderDisplayName);
            Assert.Equal(string.Empty, login.ProviderUserId);
        }

        [Fact]
        public void ExternalLogin_SetProvider_ReturnsCorrectValue()
        {
            // Arrange
            var login = new ExternalLogin();

            // Act
            login.Provider = "Google";

            // Assert
            Assert.Equal("Google", login.Provider);
        }

        [Fact]
        public void ExternalLogin_SetProviderDisplayName_ReturnsCorrectValue()
        {
            // Arrange
            var login = new ExternalLogin();

            // Act
            login.ProviderDisplayName = "Google";

            // Assert
            Assert.Equal("Google", login.ProviderDisplayName);
        }

        [Fact]
        public void ExternalLogin_SetProviderUserId_ReturnsCorrectValue()
        {
            // Arrange
            var login = new ExternalLogin();

            // Act
            login.ProviderUserId = "google-user-id-123";

            // Assert
            Assert.Equal("google-user-id-123", login.ProviderUserId);
        }

        [Fact]
        public void ExternalLogin_InitializeWithAllValues_AllPropertiesSet()
        {
            // Arrange & Act
            var login = new ExternalLogin
            {
                Provider = "Facebook",
                ProviderDisplayName = "Facebook",
                ProviderUserId = "fb-123"
            };

            // Assert
            Assert.Equal("Facebook", login.Provider);
            Assert.Equal("Facebook", login.ProviderDisplayName);
            Assert.Equal("fb-123", login.ProviderUserId);
        }
    }
}
