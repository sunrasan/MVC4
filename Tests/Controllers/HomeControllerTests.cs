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
    public class HomeControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;

        public HomeControllerTests()
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

        private void SeedAlbums()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.SaveChanges();

            for (int i = 1; i <= 8; i++)
            {
                _context.Albums.Add(new Album
                {
                    Title = $"Album {i}",
                    Price = 8.99m,
                    GenreId = genre.GenreId,
                    ArtistId = artist.ArtistId
                });
            }
            _context.SaveChanges();
        }

        [Fact]
        public async Task HomeController_Index_ReturnsViewResult()
        {
            // Arrange
            SeedAlbums();
            var controller = new HomeController(_context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task HomeController_Index_ReturnsListOfAlbums()
        {
            // Arrange
            SeedAlbums();
            var controller = new HomeController(_context);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<Album>;
            Assert.NotNull(model);
            Assert.True(model.Count <= 6);
        }

        [Fact]
        public async Task HomeController_Index_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var controller = new HomeController(_context);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<Album>;
            Assert.NotNull(model);
            Assert.Empty(model);
        }

        [Fact]
        public void HomeController_About_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(_context);

            // Act
            var result = controller.About();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeController_About_SetsViewBagMessage()
        {
            // Arrange
            var controller = new HomeController(_context);

            // Act
            controller.About();

            // Assert
            Assert.Equal("Your application description page.", controller.ViewBag.Message);
        }

        [Fact]
        public void HomeController_Contact_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(_context);

            // Act
            var result = controller.Contact();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeController_Contact_SetsViewBagMessage()
        {
            // Arrange
            var controller = new HomeController(_context);

            // Act
            controller.Contact();

            // Assert
            Assert.Equal("Your contact page.", controller.ViewBag.Message);
        }

        [Fact]
        public void HomeController_Error_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(_context);

            // Act
            var result = controller.Error();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeController_Constructor_WithValidContext_CreatesInstance()
        {
            // Act
            var controller = new HomeController(_context);

            // Assert
            Assert.NotNull(controller);
        }
    }
}
