using Ecommerce_WebApi_Application.Controllers;
using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCaRt_XUnitTest.Controllers
{
    public class OrderControllerTests
    {

        private readonly OrderController _controller;
        public OrderControllerTests()
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
            var orderDAL = new OrderDAL(configuration);

            _controller = new OrderController(configuration, orderDAL);

            // Setup the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

//Positive Test Case: To place order
        [Fact]
        public async Task PlaceOrder_ValidOrder_ReturnsOk()
        {
            // Arrange
            var orderRequest = new OrderProductModel
            {
                Customer_Id = 1,
                OrderProducts = new List<OrderProduct>
        {
            new OrderProduct { Product_Id = 1, Quantity = 2, TotalPrice = 2000 },
            new OrderProduct { Product_Id = 2, Quantity = 1, TotalPrice = 1000 }
        }
            };

            // Act
            var result = await _controller.PlaceOrder(orderRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Order placed successfully.", okResult.Value);
        }

//Negative TestCase:  Null Order Request
        [Fact]
        public async Task PlaceOrder_NullOrderRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.PlaceOrder(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid order data.", badRequestResult.Value);
        }

//Test Case for Empty Order Products
        [Fact]
        public async Task PlaceOrder_EmptyOrderProducts_ReturnsBadRequest()
        {
            // Arrange
            var orderRequest = new OrderProductModel
            {
                Customer_Id = 1,
                OrderProducts = new List<OrderProduct>()
            };

            // Act
            var result = await _controller.PlaceOrder(orderRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid order data.", badRequestResult.Value);
        }

//Positive Test Case:To get the orders based on customerid
        [Fact]
        public void GetOrderByCustomer_ValidCustomerId_ReturnsOrders()
        {
            // Arrange
            int validCustomerId = 1;

            // Act
            var result = _controller.GetOrderByCustomer(validCustomerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var ordersJson = Assert.IsType<string>(okResult.Value);
            var orders = JsonConvert.DeserializeObject<List<MyOrderModel>>(ordersJson);
            Assert.NotEmpty(orders);
        }

//Test Case for Invalid Customer ID (Zero or Negative)
        [Fact]
        public void GetOrderByCustomer_InvalidCustomerId_ReturnsBadRequest()
        {
            // Arrange
            int invalidCustomerId = 0;

            // Act
            var result = _controller.GetOrderByCustomer(invalidCustomerId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to load order.", badRequestResult.Value);
        }

//Test Case for No Orders Found
        [Fact]
        public void GetOrderByCustomer_NoOrders_ReturnsNotFound()
        {
            // Arrange
            int validCustomerIdWithNoOrders = 2; 

            // Act
            var result = _controller.GetOrderByCustomer(validCustomerIdWithNoOrders);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No order found.", notFoundResult.Value);
        }


    }
}
