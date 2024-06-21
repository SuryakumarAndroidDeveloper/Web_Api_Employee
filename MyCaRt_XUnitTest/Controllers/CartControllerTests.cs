using Ecommerce_WebApi_Application.Controllers;
using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyCaRt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCaRt_XUnitTest.Controllers
{
    public class CartControllerTests
    {
        private readonly CartController _controller;

        public CartControllerTests()
        {
            // Set up real configuration from appsettings.json or in-memory configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                {"ConnectionStrings:DefaultConnection", "Server=TISRNT03;Database=Ecommerce_Mycart;User Id=sa;Password=surya@123;TrustServerCertificate=true"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
             .AddInMemoryCollection(inMemorySettings)
             .Build();

            // Use actual implementation of DAL with real configuration
            var cartDAL = new CartDAL(configuration);

            _controller = new CartController(configuration, cartDAL);

            // Setup the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
        
//Positive TestCase:Successfully Adding an Item to the Cart
        [Fact]
        public async Task AddToCart_ValidCartItem_ReturnsOk()
        {
            // Arrange
            var validCartItem = new CartItemModel
            {
                Product_Id = 1, 
                Customer_FName = "1",
                Quantity = 5
            };

            // Act
            var result = await _controller.AddToCart(validCartItem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("cartItem updated successfully.", okResult.Value);
        }

//Negative TestCase:Null Cart Item
        [Fact]
        public async Task AddToCart_NullCartItem_ReturnsBadRequest()
        {
            // Arrange
            CartItemModel nullCartItem = null;

            // Act
            var result = await _controller.AddToCart(nullCartItem);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("cartItem are required.", badRequestResult.Value);
        }

//Negative TestCase: Failed to Add Cart Item (e.g., due to internal server error)
        [Fact]
        public async Task AddToCart_FailedToAddCartItem_ReturnsInternalServerError()
        {
            // Arrange
            var invalidCartItem = new CartItemModel
            {                
                Product_Id = 1, 
                Customer_FName = "Surya Kumar",
                Quantity = 4
            };

            // Act
            var result = await _controller.AddToCart(invalidCartItem);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error: Failed to update cartItem.", statusCodeResult.Value);
        }
//Negative TestCase: Failed to Add Cart Item productid is negative
        [Fact]
        public async Task AddToCart_negativeAddCartItem_Returns()
        {
            // Arrange
            var invalidCartItem = new CartItemModel
            {
                Product_Id = -1,
             
            };

            // Act
            var result = await _controller.AddToCart(invalidCartItem);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Add cartItem Failed.", badRequestResult.Value);
        }

//Positive Test Case: Successfully Retrieve Cart Items for a Customer
        [Fact]
        public void GetCartByCustomer_ValidCustomerId_ReturnsOk()
        {
            // Arrange
            int CustomerId = 1; 

            // Act
            var result = _controller.GetCartByCustomer(CustomerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var customerCart = Assert.IsAssignableFrom<List<DisplayCartModel>>(okResult.Value);
            Assert.NotEmpty(customerCart);
        }

//Negative Test Case: Invalid Customer ID
        [Fact]
        public void GetCartByCustomer_InvalidCustomerId_ReturnsBadRequest()
        {
            // Arrange
            int invalidCustomerId = 0;

            // Act
            var result = _controller.GetCartByCustomer(invalidCustomerId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to load Cart.", badRequestResult.Value);
        }

//Negative Test Case: No Cart Found for a Valid Customer ID

        [Fact]
        public void GetCartByCustomer_NoCartFound_ReturnsNotFound()
        {
            // Arrange
            int validCustomerIdWithNoCart = 2; 

            // Act
            var result = _controller.GetCartByCustomer(validCustomerIdWithNoCart);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No cart found.", notFoundResult.Value);
        }

//Positive Test Case: Successfully Update Cart Item Quantity
        [Fact]
        public void UpdateCartItemQuantity_ValidModel_ReturnsOk()
        {
            // Arrange
            var validModel = new UpdateCartItemQuantityModel
            {
                CartItemId = 203,
                NewQuantity = 17
            };

            // Act
            var result = _controller.UpdateCartItemQuantity(validModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Quantity updated successfully.", okResult.Value);
        }

//Negative Test Case: Null Model
        [Fact]
        public void UpdateCartItemQuantity_NullModel_ReturnsBadRequest()
        {
            // Arrange
            UpdateCartItemQuantityModel nullModel = null;

            // Act
            var result = _controller.UpdateCartItemQuantity(nullModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed", badRequestResult.Value);
        }

//Negative Test Case: Invalid Cart Item ID
        [Fact]
        public void UpdateCartItemQuantity_InvalidCartItemId_ReturnsBadRequest()
        {
            // Arrange
            var invalidModel = new UpdateCartItemQuantityModel
            {
                CartItemId = -1, 
                NewQuantity = 5
            };

            // Act
            var result = _controller.UpdateCartItemQuantity(invalidModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to update quantity.", badRequestResult.Value);
        }

//Negative Test Case: Invalid Quantity
        [Fact]
        public void UpdateCartItemQuantity_InvalidQuantity_ReturnsBadRequest()
        {
            // Arrange
            var invalidQuantityModel = new UpdateCartItemQuantityModel
            {
                CartItemId = 1, 
                NewQuantity = -5 // Invalid quantity
            };

            // Act
            var result = _controller.UpdateCartItemQuantity(invalidQuantityModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to update quantity.", badRequestResult.Value);
        }
//Positive Test Case: Successfully Delete Cart Item
        [Fact]
        public void DeleteCartItem_ValidCartItemId_ReturnsOk()
        {
            // Arrange
            var validCartItemId = 206;  

            // Act
            var result = _controller.DeleteCartItem(validCartItemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item deleted successfully.", okResult.Value);
        }

//Negative Test Case: Invalid Cart Item ID (Less than or equal to zero)
        [Fact]
        public void DeleteCartItem_InvalidCartItemId_ReturnsBadRequest()
        {
            // Arrange
            var invalidCartItemId = -1;

            // Act
            var result = _controller.DeleteCartItem(invalidCartItemId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to found item.", badRequestResult.Value);
        }

//Negative Test Case: Valid Cart Item ID but Item Not Found

        [Fact]
        public void DeleteCartItem_ValidCartItemId_ItemNotFound_ReturnsBadRequest()
        {
            // Arrange
            var nonExistentCartItemId = 9999; 

            // Act
            var result = _controller.DeleteCartItem(nonExistentCartItemId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to delete item.", badRequestResult.Value);
        }



    }
}
