using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MvcMusicStore.Controllers;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Controllers
{
    public class StoreControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;

        public StoreControllerTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        private void SeedData()
        {
            var genres = new List<Genre>
            {
                new Genre { Name = "Rock", Description = "Rock music" },
                new Genre { Name = "Jazz", Description = "Jazz music" }
            };
            _context.Genres.AddRange(genres);

            var artists = new List<Artist>
            {
                new Artist { Name = "The Beatles" },
                new Artist { Name = "Miles Davis" }
            };
            _context.Artists.AddRange(artists);
            _context.SaveChanges();

            var albums = new List<Album>
            {
                new Album { Title = "Abbey Road", Price = 8.99m, GenreId = genres[0].GenreId, ArtistId = artists[0].ArtistId },
                new Album { Title = "Kind of Blue", Price = 9.99m, GenreId = genres[1].GenreId, ArtistId = artists[1].ArtistId }
            };
            _context.Albums.AddRange(albums);
            _context.SaveChanges();
        }

        [Fact]
        public async Task StoreController_Index_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = new StoreController(_context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreController_Index_ReturnsListOfGenres()
        {
            // Arrange
            SeedData();
            var controller = new StoreController(_context);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<Genre>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task StoreController_Index_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var controller = new StoreController(_context);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<Genre>;
            Assert.NotNull(model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task StoreController_Browse_ValidGenre_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = new StoreController(_context);

            // Act
            var result = await controller.Browse("Rock");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreController_Browse_ValidGenre_ReturnsGenreModel()
        {
            // Arrange
            SeedData();
            var controller = new StoreController(_context);

            // Act
            var result = await controller.Browse("Rock") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as Genre;
            Assert.NotNull(model);
            Assert.Equal("Rock", model.Name);
        }

        [Fact]
        public async Task StoreController_Browse_InvalidGenre_ThrowsException()
        {
            // Arrange
            SeedData();
            var controller = new StoreController(_context);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => controller.Browse("NonExistentGenre"));
        }

        [Fact]
        public async Task StoreController_Details_ValidId_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = new StoreController(_context);
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await controller.Details(album.AlbumId);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreController_Details_ValidId_ReturnsAlbumModel()
        {
            // Arrange
            SeedData();
            var controller = new StoreController(_context);
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await controller.Details(album.AlbumId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as Album;
            Assert.NotNull(model);
            Assert.Equal(album.Title, model.Title);
        }

        [Fact]
        public async Task StoreController_Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            SeedData();
            var controller = new StoreController(_context);

            // Act
            var result = await controller.Details(99999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void StoreController_Constructor_WithValidContext_CreatesInstance()
        {
            // Act
            var controller = new StoreController(_context);

            // Assert
            Assert.NotNull(controller);
        }
    }
}
