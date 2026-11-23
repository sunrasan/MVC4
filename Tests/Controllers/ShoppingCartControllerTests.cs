using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MvcMusicStore.Controllers;
using MvcMusicStore.Data;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Tests.Controllers
{
    public class ShoppingCartControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;

        public ShoppingCartControllerTests()
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

        private ShoppingCartController CreateControllerWithHttpContext()
        {
            var controller = new ShoppingCartController(_context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            return controller;
        }

        private void SeedData()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "The Beatles" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.SaveChanges();

            _context.Albums.Add(new Album
            {
                Title = "Abbey Road",
                Price = 8.99m,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task ShoppingCartController_Index_ReturnsViewResult()
        {
            // Arrange
            SeedData();
            var controller = CreateControllerWithHttpContext();

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ShoppingCartController_Index_ReturnsShoppingCartViewModel()
        {
            // Arrange
            SeedData();
            var controller = CreateControllerWithHttpContext();

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ShoppingCartViewModel;
            Assert.NotNull(model);
            Assert.NotNull(model.CartItems);
        }

        [Fact]
        public async Task ShoppingCartController_Index_EmptyCart_ReturnsZeroTotal()
        {
            // Arrange
            SeedData();
            var controller = CreateControllerWithHttpContext();

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as ShoppingCartViewModel;
            Assert.NotNull(model);
            Assert.Equal(0m, model.CartTotal);
            Assert.Empty(model.CartItems);
        }

        [Fact]
        public void ShoppingCartController_Constructor_WithValidContext_CreatesInstance()
        {
            // Act
            var controller = new ShoppingCartController(_context);

            // Assert
            Assert.NotNull(controller);
        }
    }

    /// <summary>
    /// Simple in-memory session implementation for testing.
    /// </summary>
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();

        public bool IsAvailable => true;
        public string Id => "test-session-id";
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
    }
}
