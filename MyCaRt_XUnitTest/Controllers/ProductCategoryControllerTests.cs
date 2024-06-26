using Ecommerce_WebApi_Application.Controllers;
using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Google.Api.Ads.Common.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCaRt_XUnitTest.Controllers
{
    public class ProductCategoryControllerTests
    {
        private readonly ProductCategoryController _controller;



        public ProductCategoryControllerTests()
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
            var productCategoryDAL = new ProductCategoryDAL(configuration);

            _controller = new ProductCategoryController(configuration, productCategoryDAL);

            // Setup the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
//positive testcase for insert productcategory
        [Fact]
        public void InsertProductCategory_ValidModel_ReturnsOk()
        {
            // Arrange
            //string Category_Name = "Test Category" ;
            var productCategory = new ProductCategoryModel { Category_Name = "Womens FaceWash" };

            // Act
            var result = _controller.InsertProductCategory(productCategory);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("ProductCategory inserted successfully.", okResult.Value);
        }

// Negative test case for InsertProductCategory
        [Fact]
        public void InsertProductCategory_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var productCategory = new ProductCategoryModel { Category_Name = "" };

            // Act
            var result = _controller.InsertProductCategory(productCategory);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to insert ProductCategory.", badRequestResult.Value);
        }

// Negative test case for InsertProductCategory with null Category_Name
        [Fact]
        public void InsertProductCategory_NullCategoryName_ReturnsBadRequest()
        {
            // Arrange
            // Create an invalid model - for example, Category_Name is null
            var productCategory = new ProductCategoryModel { Category_Name = null };

            // Act
            var result = _controller.InsertProductCategory(productCategory);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to insert ProductCategory.", badRequestResult.Value);
        }
//postive testcase for getproduct category
        [Fact]
        public void GetProductCategory_ReturnsJson()
        {
            // Act
            var result = _controller.GetProductCategory();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var productCategories = Assert.IsAssignableFrom<List<ProductCategoryModel>>(okResult.Value);

            // Add more specific assertions based on your expected data
            Assert.NotEmpty(productCategories);
            Assert.Contains(productCategories, pc => pc.Category_Name == "Home Appliances");
            Assert.Contains(productCategories, pc => pc.Category_Name == "Electricals");


        }
//Negative Testcase for sql connection error
        [Fact]
        public void GetProductCategory_SqlException_ReturnsInternalServerError()
        {
            // Arrange: Simulate SQL connection error by setting invalid connection string
            var invalidConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                {"ConnectionStrings:DefaultConnection", "Server=TISRNT103;Database=Ecommerce_Mycart;User Id=sa;Password=surya@123;TrustServerCertificate=true"},
                })
                .Build();

            var faultyProductCategoryDAL = new ProductCategoryDAL(invalidConfig);
            var faultyController = new ProductCategoryController(invalidConfig, faultyProductCategoryDAL);

            // Act
            var result = faultyController.GetProductCategory();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("SQL Error", objectResult.Value.ToString());
        }

//Negative TestCase for storedprocedure
/*        [Fact]
        public void GetProductCategory_InvalidStoredProcedureName_ReturnsInternalServerError()
        {
            // Act
            var result = _controller.GetProductCategory("GetProductCategory123");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("SQL Error", objectResult.Value.ToString());
        }*/

//Positive TestCase for categoryname is already exists

        [Fact]
        public void IsCategoryNameAvailable_ExistingName_ReturnsTrue()
        {
            // Arrange
            var request = new ProductCategoryController.CategoryNameRequest { CategoryName = "Electricals" };

            // Act
            var result = _controller.IsCategory_NameAvailable(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            // Use reflection to access the Exists property
            var existsProperty = response.GetType().GetProperty("Exists");
            Assert.NotNull(existsProperty); // Ensure the property exists
            var existsValue = (bool)existsProperty.GetValue(response);
            Assert.True(existsValue);

        }
//Negative Testcase if the categoryname is new

        [Fact]
        public void IsCategoryNameAvailable_NonExistingName_ReturnsFalse()
        {
            // Arrange
            var request = new ProductCategoryController.CategoryNameRequest { CategoryName = "Gaming Accessiors" };

            // Act
            var result = _controller.IsCategory_NameAvailable(request);

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
