using Ecommerce_WebApi_Application.Controllers;
using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyCaRt.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCaRt_XUnitTest.Controllers
{
    public class WishListControllerTests
    {
        private readonly WishListController _controller;

        public WishListControllerTests()
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
            var wishListDAL = new WishListDAL(configuration);

            _controller = new WishListController(configuration, wishListDAL);

            // Setup the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

//Positive Test Case: Successfully Adding to Wishlist
        [Fact]
        public async Task AddToWishList_Success_ReturnsOk()
        {
            // Arrange
            var cartItem = new CartItemModel
            {
                Product_Id = 1, // Ensure this product exists in the database
                Customer_FName = "2"
                
            };

            // Act
            var result = await _controller.AddToWishList(cartItem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("cartItem updated successfully.", okResult.Value);

        }

//Negative Test Case: Null CartItem
        [Fact]
        public async Task AddToWishList_NullCartItem_ReturnsBadRequest()
        {
            // Arrange
            CartItemModel cartItem = null;

            // Act
            var result = await _controller.AddToWishList(cartItem);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("cartItem are required.", badRequestResult.Value);
        }

//Negative Test Case: Invalid Product ID

        [Fact]
        public async Task AddToWishList_InvalidProductId_ReturnsInternalServerError()
        {
            // Arrange
            var cartItem = new CartItemModel
            {
                Product_Id = -1, 
                Customer_FName = "John",
            };

            // Act
            var result = await _controller.AddToWishList(cartItem);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error: Failed to update  cartItem.", objectResult.Value);
        }
        
//Positive Test Case: Successfully Retrieving Wishlist by Customer ID
        [Fact]
        public void GetWishListByCustomer_ValidCustomerId_ReturnsOk()
        {
            // Arrange
            var CustomerId = 2; 

            // Act
            var result = _controller.GetWishListByCustomer(CustomerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var wishlistJson = Assert.IsType<string>(okResult.Value);
            var wishlist = JsonConvert.DeserializeObject<List<MyWishlistModel>>(wishlistJson);

            // Add more specific assertions based on your expected data
            Assert.NotEmpty(wishlist);
        }
//Negative Test Case: Invalid Customer ID (Zero or Negative)
        [Fact]
        public void GetWishListByCustomer_InvalidCustomerId_ReturnsBadRequest()
        {
            // Arrange
            var CustomerId = 0;

            // Act
            var result = _controller.GetWishListByCustomer(CustomerId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("CustomerId is NotFound", badRequestResult.Value);
        }

//Negative Test Case: No Wishlist Found for Valid Customer ID

        [Fact]
        public void GetWishListByCustomer_ValidCustomerIdButNoWishlist_ReturnsNotFound()
        {
            // Arrange
            var CustomerId = 4; 

            // Act
            var result = _controller.GetWishListByCustomer(CustomerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No Wishlist found.", notFoundResult.Value);
        }



    }
}
