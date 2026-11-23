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
    public class StoreManagerControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;

        public StoreManagerControllerTests()
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
            var genre = new Genre { Name = "Rock", Description = "Rock music" };
            var artist = new Artist { Name = "The Beatles" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.SaveChanges();

            var albums = new List<Album>
            {
                new Album { Title = "Abbey Road", Price = 8.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId },
                new Album { Title = "Revolver", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId }
            };
            _context.Albums.AddRange(albums);
            _context.SaveChanges();
        }

        [Fact]
        public async Task StoreManagerController_Index_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Index_ReturnsAlbumsSortedByPrice()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as List<Album>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
            Assert.True(model[0].Price <= model[1].Price);
        }

        [Fact]
        public async Task StoreManagerController_Details_ValidId_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await controller.Details(album.AlbumId);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Details_ValidId_ReturnsAlbumModel()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
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
        public async Task StoreManagerController_Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);

            // Act
            var result = await controller.Details(99999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Details_DefaultId_ReturnsNotFound()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);

            // Act
            var result = await controller.Details(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Create_GET_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);

            // Act
            var result = await controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Create_POST_ValidModel_RedirectsToIndex()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            var genre = await _context.Genres.FirstAsync();
            var artist = await _context.Artists.FirstAsync();
            var album = new Album
            {
                Title = "New Album",
                Price = 7.99m,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            };

            // Act
            var result = await controller.Create(album);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task StoreManagerController_Create_POST_ValidModel_SavesAlbum()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            var genre = await _context.Genres.FirstAsync();
            var artist = await _context.Artists.FirstAsync();
            var album = new Album
            {
                Title = "New Album",
                Price = 7.99m,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            };

            // Act
            await controller.Create(album);

            // Assert
            var saved = await _context.Albums.FirstOrDefaultAsync(a => a.Title == "New Album");
            Assert.NotNull(saved);
            Assert.Equal("New Album", saved.Title);
        }

        [Fact]
        public async Task StoreManagerController_Create_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            controller.ModelState.AddModelError("Title", "Title is required");
            var album = new Album { Price = 7.99m };

            // Act
            var result = await controller.Create(album);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Edit_GET_ValidId_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await controller.Edit(album.AlbumId);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Edit_GET_InvalidId_ReturnsNotFound()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);

            // Act
            var result = await controller.Edit(99999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Edit_POST_ValidModel_RedirectsToIndex()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            var existingAlbum = await _context.Albums.AsNoTracking().FirstAsync(a => a.Title == "Abbey Road");
            // Detach all tracked entities to avoid tracking conflict
            foreach (var entry in _context.ChangeTracker.Entries().ToList())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
            var album = new Album
            {
                AlbumId = existingAlbum.AlbumId,
                Title = "Updated Abbey Road",
                Price = 10.99m,
                GenreId = existingAlbum.GenreId,
                ArtistId = existingAlbum.ArtistId
            };

            // Act
            var result = await controller.Edit(album);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task StoreManagerController_Edit_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            controller.ModelState.AddModelError("Title", "Title is required");
            var album = new Album { Price = 7.99m };

            // Act
            var result = await controller.Edit(album);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Delete_GET_ValidId_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await controller.Delete(album.AlbumId);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_Delete_GET_InvalidId_ReturnsNotFound()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);

            // Act
            var result = await controller.Delete(99999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task StoreManagerController_DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            var album = await _context.Albums.FirstAsync();

            // Act
            var result = await controller.DeleteConfirmed(album.AlbumId);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task StoreManagerController_DeleteConfirmed_ValidId_RemovesAlbum()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);
            var album = await _context.Albums.FirstAsync();
            int albumId = album.AlbumId;

            // Act
            await controller.DeleteConfirmed(albumId);

            // Assert
            var deleted = await _context.Albums.FindAsync(albumId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task StoreManagerController_DeleteConfirmed_InvalidId_RedirectsToIndex()
        {
            // Arrange
            SeedData();
            var controller = new StoreManagerController(_context);

            // Act
            var result = await controller.DeleteConfirmed(99999);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void StoreManagerController_Constructor_WithValidContext_CreatesInstance()
        {
            // Act
            var controller = new StoreManagerController(_context);

            // Assert
            Assert.NotNull(controller);
        }
    }
}
