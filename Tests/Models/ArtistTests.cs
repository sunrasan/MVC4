using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class ArtistTests
    {
        [Fact]
        public void Artist_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var artist = new Artist();

            // Assert
            Assert.Equal(0, artist.ArtistId);
            Assert.Equal(string.Empty, artist.Name);
        }

        [Fact]
        public void Artist_SetArtistId_ReturnsCorrectValue()
        {
            // Arrange
            var artist = new Artist();

            // Act
            artist.ArtistId = 10;

            // Assert
            Assert.Equal(10, artist.ArtistId);
        }

        [Fact]
        public void Artist_SetName_ReturnsCorrectValue()
        {
            // Arrange
            var artist = new Artist();

            // Act
            artist.Name = "The Beatles";

            // Assert
            Assert.Equal("The Beatles", artist.Name);
        }

        [Fact]
        public void Artist_NameCanBeEmpty_NoException()
        {
            // Arrange & Act
            var artist = new Artist { Name = string.Empty };

            // Assert
            Assert.Equal(string.Empty, artist.Name);
        }

        [Fact]
        public void Artist_InitializeWithValues_AllPropertiesSet()
        {
            // Arrange & Act
            var artist = new Artist
            {
                ArtistId = 5,
                Name = "Metallica"
            };

            // Assert
            Assert.Equal(5, artist.ArtistId);
            Assert.Equal("Metallica", artist.Name);
        }

        [Fact]
        public void Artist_NameWithSpecialCharacters_ReturnsCorrectValue()
        {
            // Arrange
            var artist = new Artist();

            // Act
            artist.Name = "Guns N' Roses";

            // Assert
            Assert.Equal("Guns N' Roses", artist.Name);
        }

        [Fact]
        public void Artist_ArtistIdCanBeNegative_NoException()
        {
            // Arrange & Act
            var artist = new Artist { ArtistId = -1 };

            // Assert
            Assert.Equal(-1, artist.ArtistId);
        }
    }
}
