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


    public class ProductControllerTests
    {
        private readonly ProductController _controller;

        public ProductControllerTests()
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
            var productDAL = new ProductDAL(configuration);

            _controller = new ProductController(configuration, productDAL);

            // Setup the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

//positive testcase for insert product
        [Fact]
        public void InsertProduct_ValidModel_ReturnsOk()
        {
            // Arrange
            var product = new ProductModel { Product_Category="5",Product_Code="Easybuy-S1201",Product_Name="Shorts",
            Product_Description="Elastic and best quality",Product_Price=100,Available_Quantity=1000};

            // Act
            var result = _controller.InsertProduct_Details(product);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Product Added successfully.", okResult.Value);
        }

//Negative Testcase for product is empty
        [Fact]
        public void InsertProduct_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var product = new ProductModel { Product_Category="",Product_Code="",Product_Name="",
            Product_Price=null ,Product_Description="",Available_Quantity=null };

            // Act
            var result = _controller.InsertProduct_Details(product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to Add Product.", badRequestResult.Value);
        }

//Negative Testcase for product is null
        [Fact]
        public void InsertProduct_NullValue_ReturnsBadRequest()
        {
            // Arrange
            var product = new ProductModel
            {
                Product_Category = null,
                Product_Code = null,
                Product_Name = null,
                Product_Price = null,
                Product_Description = null,
                Available_Quantity = null
            };

            // Act
            var result = _controller.InsertProduct_Details(product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to Add Product.", badRequestResult.Value);
        }
//Negative Testcase for productprice and availble quantity is negative value
        [Fact]
        public void InsertProduct_NegativeValue_ReturnsBadRequest()
        {
            // Arrange
            var product = new ProductModel
            {
                Product_Category = "5",
                Product_Code = "Easybuy-S101",
                Product_Name = "shorts",
                Product_Price = -2,
                Product_Description = "Best Quality",
                Available_Quantity =  -2
            };

            // Act
            var result = _controller.InsertProduct_Details(product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to Add Product.", badRequestResult.Value);
        }
//postive testcase for getproduct category
        [Fact]
        public async Task GetAllProduct_ReturnsJson()
        {
            // Act
            var result = await _controller.GetAllProduct();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsAssignableFrom<List<ProductModel>>(okResult.Value);

            // Add more specific assertions based on your expected data
            Assert.NotEmpty(product);
        }
//Negative Testcase if the data is empty
        [Fact]
        public async Task GetAllProduct_ReturnsEmptyList()
        {
            // Act
            var result = await _controller.GetAllProduct();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsAssignableFrom<List<ProductModel>>(okResult.Value);

            // Ensure that the returned list is empty
            Assert.Empty(products);
        }
//Positive TestCase for ProductCode is already exists

       [Fact]
        public void IsProductCodeAvailable_Existing_ReturnsTrue()
        {
            // Arrange
            var request = new ProductController.ProductCodeRequest { ProductCode = "Easybuy-S101" };

            // Act
            var result = _controller.IsProduct_CodeAvailable(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            // Use reflection to access the Exists property
            var existsProperty = response.GetType().GetProperty("Exists");
            Assert.NotNull(existsProperty); // Ensure the property exists
            var existsValue = (bool)existsProperty.GetValue(response);
            Assert.True(existsValue);

        }
//Negative Testcase if the ProductCode is new

        [Fact]
        public void IsProductCodeAvailable_NonExisting_ReturnsFalse()
        {
            // Arrange
            var request = new ProductController.ProductCodeRequest { ProductCode = "Mesho-s-w12" };

            // Act
            var result = _controller.IsProduct_CodeAvailable(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value;
            // Use reflection to access the Exists property
            var existsProperty = response.GetType().GetProperty("Exists");
            Assert.NotNull(existsProperty); // Ensure the property exists
            var existsValue = (bool)existsProperty.GetValue(response);
            Assert.False(existsValue);


        }

    }
}
