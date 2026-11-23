using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class CartTests
    {
        [Fact]
        public void Cart_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var cart = new Cart();

            // Assert
            Assert.Equal(0, cart.RecordId);
            Assert.Equal(string.Empty, cart.CartId);
            Assert.Equal(0, cart.AlbumId);
            Assert.Equal(0, cart.Count);
        }

        [Fact]
        public void Cart_SetRecordId_ReturnsCorrectValue()
        {
            // Arrange
            var cart = new Cart();

            // Act
            cart.RecordId = 1;

            // Assert
            Assert.Equal(1, cart.RecordId);
        }

        [Fact]
        public void Cart_SetCartId_ReturnsCorrectValue()
        {
            // Arrange
            var cart = new Cart();

            // Act
            cart.CartId = "abc-123";

            // Assert
            Assert.Equal("abc-123", cart.CartId);
        }

        [Fact]
        public void Cart_SetAlbumId_ReturnsCorrectValue()
        {
            // Arrange
            var cart = new Cart();

            // Act
            cart.AlbumId = 5;

            // Assert
            Assert.Equal(5, cart.AlbumId);
        }

        [Fact]
        public void Cart_SetCount_ReturnsCorrectValue()
        {
            // Arrange
            var cart = new Cart();

            // Act
            cart.Count = 3;

            // Assert
            Assert.Equal(3, cart.Count);
        }

        [Fact]
        public void Cart_SetDateCreated_ReturnsCorrectValue()
        {
            // Arrange
            var cart = new Cart();
            var date = new DateTime(2024, 6, 15);

            // Act
            cart.DateCreated = date;

            // Assert
            Assert.Equal(date, cart.DateCreated);
        }

        [Fact]
        public void Cart_SetAlbum_ReturnsCorrectAlbum()
        {
            // Arrange
            var cart = new Cart();
            var album = new Album { AlbumId = 1, Title = "Test Album", Price = 9.99m };

            // Act
            cart.Album = album;

            // Assert
            Assert.NotNull(cart.Album);
            Assert.Equal("Test Album", cart.Album.Title);
            Assert.Equal(9.99m, cart.Album.Price);
        }

        [Fact]
        public void Cart_InitializeWithAllValues_AllPropertiesSet()
        {
            // Arrange & Act
            var date = DateTime.Now;
            var cart = new Cart
            {
                RecordId = 1,
                CartId = "session-guid",
                AlbumId = 10,
                Count = 2,
                DateCreated = date
            };

            // Assert
            Assert.Equal(1, cart.RecordId);
            Assert.Equal("session-guid", cart.CartId);
            Assert.Equal(10, cart.AlbumId);
            Assert.Equal(2, cart.Count);
            Assert.Equal(date, cart.DateCreated);
        }

        [Fact]
        public void Cart_CartIdCanBeGuid_NoException()
        {
            // Arrange
            var guid = Guid.NewGuid().ToString();
            var cart = new Cart { CartId = guid };

            // Assert
            Assert.Equal(guid, cart.CartId);
        }

        [Fact]
        public void Cart_CountCanBeZero_NoException()
        {
            // Arrange & Act
            var cart = new Cart { Count = 0 };

            // Assert
            Assert.Equal(0, cart.Count);
        }
    }
}
