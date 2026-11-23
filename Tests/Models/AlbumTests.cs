using System;
using System.Collections.Generic;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class AlbumTests
    {
        [Fact]
        public void Album_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var album = new Album();

            // Assert
            Assert.Equal(0, album.AlbumId);
            Assert.Equal(0, album.GenreId);
            Assert.Equal(0, album.ArtistId);
            Assert.Equal(string.Empty, album.Title);
            Assert.Equal(0m, album.Price);
            Assert.Equal(string.Empty, album.AlbumArtUrl);
            Assert.Null(album.Genre);
            Assert.Null(album.Artist);
            Assert.NotNull(album.OrderDetails);
            Assert.Empty(album.OrderDetails);
        }

        [Fact]
        public void Album_SetAlbumId_ReturnsCorrectValue()
        {
            // Arrange
            var album = new Album();

            // Act
            album.AlbumId = 42;

            // Assert
            Assert.Equal(42, album.AlbumId);
        }

        [Fact]
        public void Album_SetTitle_ReturnsCorrectValue()
        {
            // Arrange
            var album = new Album();

            // Act
            album.Title = "Abbey Road";

            // Assert
            Assert.Equal("Abbey Road", album.Title);
        }

        [Fact]
        public void Album_SetPrice_ReturnsCorrectValue()
        {
            // Arrange
            var album = new Album();

            // Act
            album.Price = 9.99m;

            // Assert
            Assert.Equal(9.99m, album.Price);
        }

        [Fact]
        public void Album_SetGenreId_ReturnsCorrectValue()
        {
            // Arrange
            var album = new Album();

            // Act
            album.GenreId = 3;

            // Assert
            Assert.Equal(3, album.GenreId);
        }

        [Fact]
        public void Album_SetArtistId_ReturnsCorrectValue()
        {
            // Arrange
            var album = new Album();

            // Act
            album.ArtistId = 7;

            // Assert
            Assert.Equal(7, album.ArtistId);
        }

        [Fact]
        public void Album_SetAlbumArtUrl_ReturnsCorrectValue()
        {
            // Arrange
            var album = new Album();

            // Act
            album.AlbumArtUrl = "/images/test.png";

            // Assert
            Assert.Equal("/images/test.png", album.AlbumArtUrl);
        }

        [Fact]
        public void Album_SetGenre_ReturnsCorrectGenre()
        {
            // Arrange
            var album = new Album();
            var genre = new Genre { GenreId = 1, Name = "Rock" };

            // Act
            album.Genre = genre;

            // Assert
            Assert.NotNull(album.Genre);
            Assert.Equal("Rock", album.Genre.Name);
        }

        [Fact]
        public void Album_SetArtist_ReturnsCorrectArtist()
        {
            // Arrange
            var album = new Album();
            var artist = new Artist { ArtistId = 1, Name = "The Beatles" };

            // Act
            album.Artist = artist;

            // Assert
            Assert.NotNull(album.Artist);
            Assert.Equal("The Beatles", album.Artist.Name);
        }

        [Fact]
        public void Album_AddOrderDetail_OrderDetailIsInList()
        {
            // Arrange
            var album = new Album();
            var orderDetail = new OrderDetail { OrderDetailId = 1, Quantity = 2 };

            // Act
            album.OrderDetails.Add(orderDetail);

            // Assert
            Assert.Single(album.OrderDetails);
            Assert.Equal(2, album.OrderDetails[0].Quantity);
        }

        [Fact]
        public void Album_InitializeWithAllValues_AllPropertiesSet()
        {
            // Arrange & Act
            var album = new Album
            {
                AlbumId = 1,
                GenreId = 2,
                ArtistId = 3,
                Title = "Test Album",
                Price = 8.99m,
                AlbumArtUrl = "/images/test.png"
            };

            // Assert
            Assert.Equal(1, album.AlbumId);
            Assert.Equal(2, album.GenreId);
            Assert.Equal(3, album.ArtistId);
            Assert.Equal("Test Album", album.Title);
            Assert.Equal(8.99m, album.Price);
            Assert.Equal("/images/test.png", album.AlbumArtUrl);
        }

        [Fact]
        public void Album_PriceCanBeZero_NoException()
        {
            // Arrange & Act
            var album = new Album { Price = 0m };

            // Assert
            Assert.Equal(0m, album.Price);
        }

        [Fact]
        public void Album_PriceCanBeMaxDecimal_NoException()
        {
            // Arrange & Act
            var album = new Album { Price = 100.00m };

            // Assert
            Assert.Equal(100.00m, album.Price);
        }
    }
}
