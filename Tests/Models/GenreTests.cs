using System;
using System.Collections.Generic;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class GenreTests
    {
        [Fact]
        public void Genre_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var genre = new Genre();

            // Assert
            Assert.Equal(0, genre.GenreId);
            Assert.Equal(string.Empty, genre.Name);
            Assert.Equal(string.Empty, genre.Description);
            Assert.NotNull(genre.Albums);
            Assert.Empty(genre.Albums);
        }

        [Fact]
        public void Genre_SetGenreId_ReturnsCorrectValue()
        {
            // Arrange
            var genre = new Genre();

            // Act
            genre.GenreId = 5;

            // Assert
            Assert.Equal(5, genre.GenreId);
        }

        [Fact]
        public void Genre_SetName_ReturnsCorrectValue()
        {
            // Arrange
            var genre = new Genre();

            // Act
            genre.Name = "Rock";

            // Assert
            Assert.Equal("Rock", genre.Name);
        }

        [Fact]
        public void Genre_SetDescription_ReturnsCorrectValue()
        {
            // Arrange
            var genre = new Genre();

            // Act
            genre.Description = "Rock music genre";

            // Assert
            Assert.Equal("Rock music genre", genre.Description);
        }

        [Fact]
        public void Genre_AddAlbumToList_AlbumIsInList()
        {
            // Arrange
            var genre = new Genre();
            var album = new Album { AlbumId = 1, Title = "Test Album" };

            // Act
            genre.Albums.Add(album);

            // Assert
            Assert.Single(genre.Albums);
            Assert.Equal("Test Album", genre.Albums[0].Title);
        }

        [Fact]
        public void Genre_SetAlbumsList_ReturnsCorrectList()
        {
            // Arrange
            var genre = new Genre();
            var albums = new List<Album>
            {
                new Album { AlbumId = 1, Title = "Album 1" },
                new Album { AlbumId = 2, Title = "Album 2" }
            };

            // Act
            genre.Albums = albums;

            // Assert
            Assert.Equal(2, genre.Albums.Count);
        }

        [Fact]
        public void Genre_NameCanBeEmpty_NoException()
        {
            // Arrange & Act
            var genre = new Genre { Name = string.Empty };

            // Assert
            Assert.Equal(string.Empty, genre.Name);
        }

        [Fact]
        public void Genre_DescriptionCanBeEmpty_NoException()
        {
            // Arrange & Act
            var genre = new Genre { Description = string.Empty };

            // Assert
            Assert.Equal(string.Empty, genre.Description);
        }

        [Fact]
        public void Genre_InitializeWithValues_AllPropertiesSet()
        {
            // Arrange & Act
            var genre = new Genre
            {
                GenreId = 10,
                Name = "Jazz",
                Description = "Jazz music",
                Albums = new List<Album>()
            };

            // Assert
            Assert.Equal(10, genre.GenreId);
            Assert.Equal("Jazz", genre.Name);
            Assert.Equal("Jazz music", genre.Description);
            Assert.NotNull(genre.Albums);
        }
    }
}
