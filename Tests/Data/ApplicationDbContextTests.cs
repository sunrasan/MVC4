using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Data
{
    public class ApplicationDbContextTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextTests()
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

        [Fact]
        public void ApplicationDbContext_Constructor_CreatesInstance()
        {
            // Assert
            Assert.NotNull(_context);
        }

        [Fact]
        public async Task ApplicationDbContext_AddAlbum_AlbumIsSaved()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock music" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album
            {
                Title = "Test Album",
                Price = 9.99m,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            };

            // Act
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Albums.FindAsync(album.AlbumId);
            Assert.NotNull(saved);
            Assert.Equal("Test Album", saved.Title);
        }

        [Fact]
        public async Task ApplicationDbContext_AddGenre_GenreIsSaved()
        {
            // Arrange
            var genre = new Genre { Name = "Rock", Description = "Rock music" };

            // Act
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Genres.FindAsync(genre.GenreId);
            Assert.NotNull(saved);
            Assert.Equal("Rock", saved.Name);
        }

        [Fact]
        public async Task ApplicationDbContext_AddArtist_ArtistIsSaved()
        {
            // Arrange
            var artist = new Artist { Name = "The Beatles" };

            // Act
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Artists.FindAsync(artist.ArtistId);
            Assert.NotNull(saved);
            Assert.Equal("The Beatles", saved.Name);
        }

        [Fact]
        public async Task ApplicationDbContext_AddCart_CartIsSaved()
        {
            // Arrange
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Test", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            var cart = new Cart
            {
                CartId = "test-cart-id",
                AlbumId = album.AlbumId,
                Count = 2,
                DateCreated = DateTime.Now
            };

            // Act
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Carts.FindAsync(cart.RecordId);
            Assert.NotNull(saved);
            Assert.Equal("test-cart-id", saved.CartId);
            Assert.Equal(2, saved.Count);
        }

        [Fact]
        public async Task ApplicationDbContext_AddOrder_OrderIsSaved()
        {
            // Arrange
            var order = new Order
            {
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "USA",
                Phone = "555-1234",
                Email = "john@example.com",
                Total = 9.99m,
                OrderDate = DateTime.Now
            };

            // Act
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Orders.FindAsync(order.OrderId);
            Assert.NotNull(saved);
            Assert.Equal("testuser", saved.Username);
            Assert.Equal(9.99m, saved.Total);
        }

        [Fact]
        public async Task ApplicationDbContext_AddOrderDetail_OrderDetailIsSaved()
        {
            // Arrange
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Test", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);

            var order = new Order
            {
                Username = "user",
                FirstName = "A",
                LastName = "B",
                Address = "123 St",
                City = "City",
                State = "ST",
                PostalCode = "12345",
                Country = "USA",
                Phone = "555-0000",
                Email = "a@b.com",
                OrderDate = DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                AlbumId = album.AlbumId,
                Quantity = 2,
                UnitPrice = 8.99m
            };

            // Act
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.OrderDetails.FindAsync(orderDetail.OrderDetailId);
            Assert.NotNull(saved);
            Assert.Equal(2, saved.Quantity);
            Assert.Equal(8.99m, saved.UnitPrice);
        }

        [Fact]
        public async Task ApplicationDbContext_RemoveAlbum_AlbumIsDeleted()
        {
            // Arrange
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "To Delete", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            int albumId = album.AlbumId;

            // Act
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            // Assert
            var deleted = await _context.Albums.FindAsync(albumId);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task ApplicationDbContext_UpdateAlbum_AlbumIsUpdated()
        {
            // Arrange
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Test Artist" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();

            var album = new Album { Title = "Original Title", Price = 9.99m, GenreId = genre.GenreId, ArtistId = artist.ArtistId };
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();

            // Act
            album.Title = "Updated Title";
            _context.Update(album);
            await _context.SaveChangesAsync();

            // Assert
            var updated = await _context.Albums.FindAsync(album.AlbumId);
            Assert.NotNull(updated);
            Assert.Equal("Updated Title", updated.Title);
        }

        [Fact]
        public void ApplicationDbContext_DbSets_AreNotNull()
        {
            // Assert
            Assert.NotNull(_context.Albums);
            Assert.NotNull(_context.Genres);
            Assert.NotNull(_context.Artists);
            Assert.NotNull(_context.Carts);
            Assert.NotNull(_context.Orders);
            Assert.NotNull(_context.OrderDetails);
        }
    }
}
