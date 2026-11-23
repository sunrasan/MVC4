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

namespace MvcMusicStore.Tests.Controllers
{
    public class CheckoutControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;

        public CheckoutControllerTests()
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

        private CheckoutController CreateControllerWithHttpContext()
        {
            var controller = new CheckoutController(_context);
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            return controller;
        }

        [Fact]
        public void CheckoutController_AddressAndPayment_GET_ReturnsViewResult()
        {
            // Arrange
            var controller = CreateControllerWithHttpContext();

            // Act
            var result = controller.AddressAndPayment();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CheckoutController_AddressAndPayment_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            var controller = CreateControllerWithHttpContext();
            controller.ModelState.AddModelError("FirstName", "First name is required");
            var order = new Order();

            // Act
            var result = await controller.AddressAndPayment(order, null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CheckoutController_AddressAndPayment_POST_InvalidPromoCode_ReturnsView()
        {
            // Arrange
            var controller = CreateControllerWithHttpContext();
            var order = CreateValidOrder();

            // Act
            var result = await controller.AddressAndPayment(order, "INVALID");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CheckoutController_AddressAndPayment_POST_InvalidPromoCode_AddsModelError()
        {
            // Arrange
            var controller = CreateControllerWithHttpContext();
            var order = CreateValidOrder();

            // Act
            await controller.AddressAndPayment(order, "WRONGCODE");

            // Assert
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task CheckoutController_AddressAndPayment_POST_NullPromoCode_ReturnsView()
        {
            // Arrange
            var controller = CreateControllerWithHttpContext();
            var order = CreateValidOrder();

            // Act
            var result = await controller.AddressAndPayment(order, null);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CheckoutController_AddressAndPayment_POST_EmptyPromoCode_ReturnsView()
        {
            // Arrange
            var controller = CreateControllerWithHttpContext();
            var order = CreateValidOrder();

            // Act
            var result = await controller.AddressAndPayment(order, "");

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void CheckoutController_Constructor_WithValidContext_CreatesInstance()
        {
            // Act
            var controller = new CheckoutController(_context);

            // Assert
            Assert.NotNull(controller);
        }

        private Order CreateValidOrder()
        {
            return new Order
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "USA",
                Phone = "555-1234",
                Email = "john@example.com"
            };
        }
    }
}
