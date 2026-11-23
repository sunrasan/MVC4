using System;
using System.Collections.Generic;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class OrderTests
    {
        [Fact]
        public void Order_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            Assert.Equal(0, order.OrderId);
            Assert.Equal(string.Empty, order.Username);
            Assert.Equal(string.Empty, order.FirstName);
            Assert.Equal(string.Empty, order.LastName);
            Assert.Equal(string.Empty, order.Address);
            Assert.Equal(string.Empty, order.City);
            Assert.Equal(string.Empty, order.State);
            Assert.Equal(string.Empty, order.PostalCode);
            Assert.Equal(string.Empty, order.Country);
            Assert.Equal(string.Empty, order.Phone);
            Assert.Equal(string.Empty, order.Email);
            Assert.Equal(0m, order.Total);
            Assert.NotNull(order.OrderDetails);
            Assert.Empty(order.OrderDetails);
        }

        [Fact]
        public void Order_SetOrderId_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.OrderId = 100;

            // Assert
            Assert.Equal(100, order.OrderId);
        }

        [Fact]
        public void Order_SetUsername_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.Username = "testuser";

            // Assert
            Assert.Equal("testuser", order.Username);
        }

        [Fact]
        public void Order_SetFirstName_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.FirstName = "John";

            // Assert
            Assert.Equal("John", order.FirstName);
        }

        [Fact]
        public void Order_SetLastName_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.LastName = "Doe";

            // Assert
            Assert.Equal("Doe", order.LastName);
        }

        [Fact]
        public void Order_SetAddress_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.Address = "123 Main St";

            // Assert
            Assert.Equal("123 Main St", order.Address);
        }

        [Fact]
        public void Order_SetCity_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.City = "Springfield";

            // Assert
            Assert.Equal("Springfield", order.City);
        }

        [Fact]
        public void Order_SetState_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.State = "IL";

            // Assert
            Assert.Equal("IL", order.State);
        }

        [Fact]
        public void Order_SetPostalCode_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.PostalCode = "62701";

            // Assert
            Assert.Equal("62701", order.PostalCode);
        }

        [Fact]
        public void Order_SetCountry_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.Country = "USA";

            // Assert
            Assert.Equal("USA", order.Country);
        }

        [Fact]
        public void Order_SetPhone_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.Phone = "555-1234";

            // Assert
            Assert.Equal("555-1234", order.Phone);
        }

        [Fact]
        public void Order_SetEmail_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.Email = "test@example.com";

            // Assert
            Assert.Equal("test@example.com", order.Email);
        }

        [Fact]
        public void Order_SetTotal_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();

            // Act
            order.Total = 49.99m;

            // Assert
            Assert.Equal(49.99m, order.Total);
        }

        [Fact]
        public void Order_SetOrderDate_ReturnsCorrectValue()
        {
            // Arrange
            var order = new Order();
            var date = new DateTime(2024, 1, 15);

            // Act
            order.OrderDate = date;

            // Assert
            Assert.Equal(date, order.OrderDate);
        }

        [Fact]
        public void Order_AddOrderDetail_OrderDetailIsInList()
        {
            // Arrange
            var order = new Order();
            var detail = new OrderDetail { OrderDetailId = 1, Quantity = 2 };

            // Act
            order.OrderDetails.Add(detail);

            // Assert
            Assert.Single(order.OrderDetails);
            Assert.Equal(2, order.OrderDetails[0].Quantity);
        }

        [Fact]
        public void Order_InitializeWithAllValues_AllPropertiesSet()
        {
            // Arrange & Act
            var order = new Order
            {
                OrderId = 1,
                Username = "user1",
                FirstName = "Jane",
                LastName = "Smith",
                Address = "456 Oak Ave",
                City = "Chicago",
                State = "IL",
                PostalCode = "60601",
                Country = "USA",
                Phone = "312-555-0100",
                Email = "jane@example.com",
                Total = 29.99m
            };

            // Assert
            Assert.Equal(1, order.OrderId);
            Assert.Equal("user1", order.Username);
            Assert.Equal("Jane", order.FirstName);
            Assert.Equal("Smith", order.LastName);
            Assert.Equal("456 Oak Ave", order.Address);
            Assert.Equal("Chicago", order.City);
            Assert.Equal("IL", order.State);
            Assert.Equal("60601", order.PostalCode);
            Assert.Equal("USA", order.Country);
            Assert.Equal("312-555-0100", order.Phone);
            Assert.Equal("jane@example.com", order.Email);
            Assert.Equal(29.99m, order.Total);
        }
    }
}
