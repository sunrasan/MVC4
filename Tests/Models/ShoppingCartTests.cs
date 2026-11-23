using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class ShoppingCartTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;

        public ShoppingCartTests()
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

        private void SeedAlbum(int albumId, decimal price = 9.99m)
        {
            var genre = new Genre { Name = $"Genre{albumId}" };
            var artist = new Artist { Name = $"Artist{albumId}" };
            _context.Genres.Add(genre);
            _context.Artists.Add(artist);
            _context.SaveChanges();

            var album = new Album
            {
                Title = $"Album {albumId}",
                Price = price,
                GenreId = genre.GenreId,
                ArtistId = artist.ArtistId
            };
            _context.Albums.Add(album);
            _context.SaveChanges();
        }

        private ShoppingCart CreateCartWithId(string cartId)
        {
            var cart = new ShoppingCart(_context);
            var prop = typeof(ShoppingCart).GetProperty("ShoppingCartId",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop?.SetValue(cart, cartId);
            return cart;
        }

        [Fact]
        public void ShoppingCart_Constructor_CreatesInstance()
        {
            // Act
            var cart = new ShoppingCart(_context);

            // Assert
            Assert.NotNull(cart);
        }

        [Fact]
        public void ShoppingCart_CartSessionKey_IsCorrectValue()
        {
            // Assert
            Assert.Equal("CartId", ShoppingCart.CartSessionKey);
        }

        [Fact]
        public void ShoppingCart_AddToCart_NewItem_AddsCartEntry()
        {
            // Arrange
            SeedAlbum(1);
            var album = _context.Albums.First();
            var cart = CreateCartWithId("test-cart-id");

            // Act
            cart.AddToCart(album);
            _context.SaveChanges();

            // Assert
            var cartItems = _context.Carts.Where(c => c.CartId == "test-cart-id").ToList();
            Assert.Single(cartItems);
            Assert.Equal(1, cartItems[0].Count);
            Assert.Equal(album.AlbumId, cartItems[0].AlbumId);
        }

        [Fact]
        public void ShoppingCart_AddToCart_ExistingItem_IncrementsCount()
        {
            // Arrange
            SeedAlbum(1);
            var album = _context.Albums.First();
            var cart = CreateCartWithId("test-cart-id");

            // Act
            cart.AddToCart(album);
            _context.SaveChanges();
            cart.AddToCart(album);
            _context.SaveChanges();

            // Assert
            var cartItems = _context.Carts.Where(c => c.CartId == "test-cart-id").ToList();
            Assert.Single(cartItems);
            Assert.Equal(2, cartItems[0].Count);
        }

        [Fact]
        public void ShoppingCart_RemoveFromCart_CountGreaterThanOne_DecrementsCount()
        {
            // Arrange
            SeedAlbum(1);
            var album = _context.Albums.First();

            var cartItem = new Cart
            {
                CartId = "test-cart-id",
                AlbumId = album.AlbumId,
                Count = 3,
                DateCreated = DateTime.Now
            };
            _context.Carts.Add(cartItem);
            _context.SaveChanges();

            var cart = CreateCartWithId("test-cart-id");

            // Act
            int itemCount = cart.RemoveFromCart(cartItem.RecordId);
            _context.SaveChanges();

            // Assert
            Assert.Equal(2, itemCount);
            var remaining = _context.Carts.Find(cartItem.RecordId);
            Assert.NotNull(remaining);
            Assert.Equal(2, remaining.Count);
        }

        [Fact]
        public void ShoppingCart_RemoveFromCart_CountEqualsOne_RemovesItem()
        {
            // Arrange
            SeedAlbum(1);
            var album = _context.Albums.First();

            var cartItem = new Cart
            {
                CartId = "test-cart-id",
                AlbumId = album.AlbumId,
                Count = 1,
                DateCreated = DateTime.Now
            };
            _context.Carts.Add(cartItem);
            _context.SaveChanges();
            int recordId = cartItem.RecordId;

            var cart = CreateCartWithId("test-cart-id");

            // Act
            int itemCount = cart.RemoveFromCart(recordId);
            _context.SaveChanges();

            // Assert
            Assert.Equal(0, itemCount);
            var removed = _context.Carts.Find(recordId);
            Assert.Null(removed);
        }

        [Fact]
        public void ShoppingCart_EmptyCart_RemovesAllItems()
        {
            // Arrange
            SeedAlbum(1);
            SeedAlbum(2);
            var albums = _context.Albums.ToList();

            _context.Carts.AddRange(
                new Cart { CartId = "test-cart-id", AlbumId = albums[0].AlbumId, Count = 2, DateCreated = DateTime.Now },
                new Cart { CartId = "test-cart-id", AlbumId = albums[1].AlbumId, Count = 1, DateCreated = DateTime.Now }
            );
            _context.SaveChanges();

            var cart = CreateCartWithId("test-cart-id");

            // Act
            cart.EmptyCart();
            _context.SaveChanges();

            // Assert
            var remaining = _context.Carts.Where(c => c.CartId == "test-cart-id").ToList();
            Assert.Empty(remaining);
        }

        [Fact]
        public void ShoppingCart_GetCartItems_ReturnsCorrectItems()
        {
            // Arrange
            SeedAlbum(1);
            SeedAlbum(2);
            var albums = _context.Albums.ToList();

            _context.Carts.AddRange(
                new Cart { CartId = "test-cart-id", AlbumId = albums[0].AlbumId, Count = 1, DateCreated = DateTime.Now },
                new Cart { CartId = "test-cart-id", AlbumId = albums[1].AlbumId, Count = 2, DateCreated = DateTime.Now },
                new Cart { CartId = "other-cart-id", AlbumId = albums[0].AlbumId, Count = 1, DateCreated = DateTime.Now }
            );
            _context.SaveChanges();

            var cart = CreateCartWithId("test-cart-id");

            // Act
            var items = cart.GetCartItems();

            // Assert
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public void ShoppingCart_GetCount_ReturnsCorrectCount()
        {
            // Arrange
            SeedAlbum(1);
            SeedAlbum(2);
            var albums = _context.Albums.ToList();

            _context.Carts.AddRange(
                new Cart { CartId = "test-cart-id", AlbumId = albums[0].AlbumId, Count = 3, DateCreated = DateTime.Now },
                new Cart { CartId = "test-cart-id", AlbumId = albums[1].AlbumId, Count = 2, DateCreated = DateTime.Now }
            );
            _context.SaveChanges();

            var cart = CreateCartWithId("test-cart-id");

            // Act
            int count = cart.GetCount();

            // Assert
            Assert.Equal(5, count);
        }

        [Fact]
        public void ShoppingCart_GetCount_EmptyCart_ReturnsZero()
        {
            // Arrange
            var cart = CreateCartWithId("empty-cart-id");

            // Act
            int count = cart.GetCount();

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void ShoppingCart_MigrateCart_UpdatesCartIdToUserName()
        {
            // Arrange
            SeedAlbum(1);
            var album = _context.Albums.First();

            _context.Carts.Add(new Cart
            {
                CartId = "anonymous-cart-id",
                AlbumId = album.AlbumId,
                Count = 1,
                DateCreated = DateTime.Now
            });
            _context.SaveChanges();

            var cart = CreateCartWithId("anonymous-cart-id");

            // Act
            cart.MigrateCart("newuser");
            _context.SaveChanges();

            // Assert
            var migratedItems = _context.Carts.Where(c => c.CartId == "newuser").ToList();
            Assert.Single(migratedItems);
        }

        [Fact]
        public void ShoppingCart_GetCartItems_EmptyCart_ReturnsEmptyList()
        {
            // Arrange
            var cart = CreateCartWithId("empty-cart-id");

            // Act
            var items = cart.GetCartItems();

            // Assert
            Assert.Empty(items);
        }
    }
}
