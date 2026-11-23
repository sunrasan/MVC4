using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class OrderDetailTests
    {
        [Fact]
        public void OrderDetail_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var detail = new OrderDetail();

            // Assert
            Assert.Equal(0, detail.OrderDetailId);
            Assert.Equal(0, detail.OrderId);
            Assert.Equal(0, detail.AlbumId);
            Assert.Equal(0, detail.Quantity);
            Assert.Equal(0m, detail.UnitPrice);
        }

        [Fact]
        public void OrderDetail_SetOrderDetailId_ReturnsCorrectValue()
        {
            // Arrange
            var detail = new OrderDetail();

            // Act
            detail.OrderDetailId = 5;

            // Assert
            Assert.Equal(5, detail.OrderDetailId);
        }

        [Fact]
        public void OrderDetail_SetOrderId_ReturnsCorrectValue()
        {
            // Arrange
            var detail = new OrderDetail();

            // Act
            detail.OrderId = 10;

            // Assert
            Assert.Equal(10, detail.OrderId);
        }

        [Fact]
        public void OrderDetail_SetAlbumId_ReturnsCorrectValue()
        {
            // Arrange
            var detail = new OrderDetail();

            // Act
            detail.AlbumId = 20;

            // Assert
            Assert.Equal(20, detail.AlbumId);
        }

        [Fact]
        public void OrderDetail_SetQuantity_ReturnsCorrectValue()
        {
            // Arrange
            var detail = new OrderDetail();

            // Act
            detail.Quantity = 3;

            // Assert
            Assert.Equal(3, detail.Quantity);
        }

        [Fact]
        public void OrderDetail_SetUnitPrice_ReturnsCorrectValue()
        {
            // Arrange
            var detail = new OrderDetail();

            // Act
            detail.UnitPrice = 8.99m;

            // Assert
            Assert.Equal(8.99m, detail.UnitPrice);
        }

        [Fact]
        public void OrderDetail_SetAlbum_ReturnsCorrectAlbum()
        {
            // Arrange
            var detail = new OrderDetail();
            var album = new Album { AlbumId = 1, Title = "Test Album" };

            // Act
            detail.Album = album;

            // Assert
            Assert.NotNull(detail.Album);
            Assert.Equal("Test Album", detail.Album.Title);
        }

        [Fact]
        public void OrderDetail_SetOrder_ReturnsCorrectOrder()
        {
            // Arrange
            var detail = new OrderDetail();
            var order = new Order { OrderId = 1, Username = "testuser" };

            // Act
            detail.Order = order;

            // Assert
            Assert.NotNull(detail.Order);
            Assert.Equal("testuser", detail.Order.Username);
        }

        [Fact]
        public void OrderDetail_InitializeWithAllValues_AllPropertiesSet()
        {
            // Arrange & Act
            var detail = new OrderDetail
            {
                OrderDetailId = 1,
                OrderId = 2,
                AlbumId = 3,
                Quantity = 4,
                UnitPrice = 9.99m
            };

            // Assert
            Assert.Equal(1, detail.OrderDetailId);
            Assert.Equal(2, detail.OrderId);
            Assert.Equal(3, detail.AlbumId);
            Assert.Equal(4, detail.Quantity);
            Assert.Equal(9.99m, detail.UnitPrice);
        }

        [Fact]
        public void OrderDetail_UnitPriceCanBeZero_NoException()
        {
            // Arrange & Act
            var detail = new OrderDetail { UnitPrice = 0m };

            // Assert
            Assert.Equal(0m, detail.UnitPrice);
        }
    }
}
