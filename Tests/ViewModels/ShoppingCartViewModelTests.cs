using System;
using System.Collections.Generic;
using Xunit;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Tests.ViewModels
{
    public class ShoppingCartViewModelTests
    {
        [Fact]
        public void ShoppingCartViewModel_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var viewModel = new ShoppingCartViewModel();

            // Assert
            Assert.NotNull(viewModel.CartItems);
            Assert.Empty(viewModel.CartItems);
            Assert.Equal(0m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_SetCartTotal_ReturnsCorrectValue()
        {
            // Arrange
            var viewModel = new ShoppingCartViewModel();

            // Act
            viewModel.CartTotal = 49.99m;

            // Assert
            Assert.Equal(49.99m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_SetCartItems_ReturnsCorrectList()
        {
            // Arrange
            var viewModel = new ShoppingCartViewModel();
            var items = new List<Cart>
            {
                new Cart { RecordId = 1, CartId = "cart1", AlbumId = 1, Count = 2 },
                new Cart { RecordId = 2, CartId = "cart1", AlbumId = 2, Count = 1 }
            };

            // Act
            viewModel.CartItems = items;

            // Assert
            Assert.Equal(2, viewModel.CartItems.Count);
        }

        [Fact]
        public void ShoppingCartViewModel_AddCartItem_ItemIsInList()
        {
            // Arrange
            var viewModel = new ShoppingCartViewModel();
            var item = new Cart { RecordId = 1, CartId = "cart1", AlbumId = 1, Count = 1 };

            // Act
            viewModel.CartItems.Add(item);

            // Assert
            Assert.Single(viewModel.CartItems);
        }

        [Fact]
        public void ShoppingCartViewModel_CartTotalCanBeZero_NoException()
        {
            // Arrange & Act
            var viewModel = new ShoppingCartViewModel { CartTotal = 0m };

            // Assert
            Assert.Equal(0m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_InitializeWithValues_AllPropertiesSet()
        {
            // Arrange
            var items = new List<Cart> { new Cart { RecordId = 1 } };

            // Act
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = items,
                CartTotal = 8.99m
            };

            // Assert
            Assert.Single(viewModel.CartItems);
            Assert.Equal(8.99m, viewModel.CartTotal);
        }
    }

    public class ShoppingCartRemoveViewModelTests
    {
        [Fact]
        public void ShoppingCartRemoveViewModel_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var viewModel = new ShoppingCartRemoveViewModel();

            // Assert
            Assert.Equal(string.Empty, viewModel.Message);
            Assert.Equal(0m, viewModel.CartTotal);
            Assert.Equal(0, viewModel.CartCount);
            Assert.Equal(0, viewModel.ItemCount);
            Assert.Equal(0, viewModel.DeleteId);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetMessage_ReturnsCorrectValue()
        {
            // Arrange
            var viewModel = new ShoppingCartRemoveViewModel();

            // Act
            viewModel.Message = "Item removed from cart.";

            // Assert
            Assert.Equal("Item removed from cart.", viewModel.Message);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetCartTotal_ReturnsCorrectValue()
        {
            // Arrange
            var viewModel = new ShoppingCartRemoveViewModel();

            // Act
            viewModel.CartTotal = 17.98m;

            // Assert
            Assert.Equal(17.98m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetCartCount_ReturnsCorrectValue()
        {
            // Arrange
            var viewModel = new ShoppingCartRemoveViewModel();

            // Act
            viewModel.CartCount = 3;

            // Assert
            Assert.Equal(3, viewModel.CartCount);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetItemCount_ReturnsCorrectValue()
        {
            // Arrange
            var viewModel = new ShoppingCartRemoveViewModel();

            // Act
            viewModel.ItemCount = 2;

            // Assert
            Assert.Equal(2, viewModel.ItemCount);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetDeleteId_ReturnsCorrectValue()
        {
            // Arrange
            var viewModel = new ShoppingCartRemoveViewModel();

            // Act
            viewModel.DeleteId = 42;

            // Assert
            Assert.Equal(42, viewModel.DeleteId);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_InitializeWithAllValues_AllPropertiesSet()
        {
            // Arrange & Act
            var viewModel = new ShoppingCartRemoveViewModel
            {
                Message = "Album removed.",
                CartTotal = 26.97m,
                CartCount = 3,
                ItemCount = 1,
                DeleteId = 5
            };

            // Assert
            Assert.Equal("Album removed.", viewModel.Message);
            Assert.Equal(26.97m, viewModel.CartTotal);
            Assert.Equal(3, viewModel.CartCount);
            Assert.Equal(1, viewModel.ItemCount);
            Assert.Equal(5, viewModel.DeleteId);
        }
    }
}
