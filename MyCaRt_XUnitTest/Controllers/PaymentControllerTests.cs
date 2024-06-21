using Ecommerce_WebApi_Application.Controllers;
using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCaRt_XUnitTest.Controllers
{
    public class PaymentControllerTests
    {

        private readonly PaymentController _controller;
        public PaymentControllerTests()
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
            var paymentDAL = new PaymentDAL(configuration);

            _controller = new PaymentController(configuration, paymentDAL);

            // Setup the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }


//Positive Test Case: Successfully Add Payment
        [Fact]
        public async Task AddPayment_ValidPaymentModel_ReturnsOk()
        {
            // Arrange
            var paymentModel = new PaymentModel
            {
                CustomerId = 1, 
                FullName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                CardName = "John Doe",
                CardNumber = "4111111111111111",
                ExpMonth = "12",
                ExpYear = "2025",
                CVV = "123"
            };

            // Act
            var result = await _controller.AddPayment(paymentModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment stored successfully!", okResult.Value);
        }

//Negative Test Case: Invalid Payment Model (ModelState Invalid)

        [Fact]
        public async Task AddPayment_InvalidPaymentModel_ReturnsBadRequest()
        {
            // Arrange
            var paymentModel = new PaymentModel
            {
                CustomerId = 1,
     
                Email = "johndoe@example.com",
                Address = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                CardName = "John Doe",
                CardNumber = "4111111111111111",
                ExpMonth = "12",
                ExpYear = "2025",
                CVV = "123"
            };

            // Act
            var result = await _controller.AddPayment(paymentModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to store the payment.", badRequestResult.Value);
        }
        }

    }

