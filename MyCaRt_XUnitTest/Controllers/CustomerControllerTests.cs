using Castle.Core.Resource;
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
    public class CustomerControllerTests
    {
        private readonly CustomerController _controller;


        public CustomerControllerTests()
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
            var customerDAL = new CustomerDAL(configuration);

            _controller = new CustomerController(configuration, customerDAL);

            // Setup the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

//Positive Testcase to insert the customer details
        [Fact]
        public async Task InsertCustomerDetails_Success_ReturnsOk()
        {
            // Arrange
            var customer = new CustomerModel
            {
                Customer_FName = "John",
                Customer_LName = "Doe",
                Customer_Gender = "Male",
                Customer_Email = "john.doe@example.com",
                Customer_Mobile = "1234567890",
                SelectedAreas = new List<int> { 1, 2, 3 }


            };

            try
            {
                // Act
                var result = await _controller.InsertCustomerDetails(customer);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal("Customer details inserted successfully.", okResult.Value);
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                throw;
            }
        }
//Negative Testcase invalid modelstate
        [Fact]
        public async Task InsertCustomerDetails_InvalidData_ReturnsInternalServerError()
        {
             //Arrange
            var customer = new CustomerModel
            {
              
                Customer_LName = "Doe",
                Customer_Gender = "Male",
                Customer_Email = "john.doe@example.com",
                Customer_Mobile = "1234567890",
                SelectedAreas = new List<int> { 1, 2, 3 }
            };

            try
            {
                // Act
                var result = await _controller.InsertCustomerDetails(customer);

                // Assert
                var objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal(500, objectResult.StatusCode);
                Assert.Equal("Internal server error: Failed to insert customer details.", objectResult.Value);

            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                throw;
            }
        }
//Positive TestCase for getting all the customer

        [Fact]
        public async Task GetAllCustomer_ReturnsCustomers()
        {
   
            // Act
            var result = await _controller.GetAllCustomer();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomers = Assert.IsAssignableFrom<List<CustomerInterestedCategory>>(okResult.Value);

            Assert.NotEmpty(returnedCustomers);
            
        }

//Positive Testcase for getting a customer based in the id
        [Fact]
        public async Task GetCustomerByID_ExistingCustomer_ReturnsOk()
        {
            // Arrange
            var id = 2; // Assuming this ID exists in the database
            var customer = new CustomerInterestedCategory
            {
                Customer_Id = id,
                Customer_FName = "Surya kumar",
                Customer_LName = "Radhakrishnan",
                Customer_Gender = "Male",
                Customer_Email ="surya@gmail.com",
                Customer_Mobile="6382421795",
                Customer_InterestedCategory= "Home Appliances, Men"

            };
            // Act
            var result = await _controller.GetCustomerByID(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomer = Assert.IsType<CustomerInterestedCategory>(okResult.Value);

            Assert.Equal(id, returnedCustomer.Customer_Id);
            Assert.Equal(customer.Customer_FName, returnedCustomer.Customer_FName);
            Assert.Equal(customer.Customer_LName, returnedCustomer.Customer_LName);
            Assert.Equal(customer.Customer_Gender, returnedCustomer.Customer_Gender);
            Assert.Equal(customer.Customer_Email, returnedCustomer.Customer_Email);
            Assert.Equal(customer.Customer_Mobile, returnedCustomer.Customer_Mobile);
            Assert.Equal(customer.Customer_InterestedCategory, returnedCustomer.Customer_InterestedCategory);

        }
//Negative TestCase if customer id is null

        [Fact]
        public async Task GetCustomerByID_NullCustomer_ReturnsBadRequest()
        {
            // Arrange
            var id = -1; 
       
            // Act
            var result = await _controller.GetCustomerByID(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Fail to get Customer", badRequestResult.Value);
        }
//Negative Testcase if id is not present in db
        [Fact]
        public async Task GetCustomerByID_NonExistingCustomer_ReturnsNotFound()
        {
            // Arrange
            var nonExistingCustomerId = 999; // Assuming this ID does not exist in the database

            // Act
            var result = await _controller.GetCustomerByID(nonExistingCustomerId);

            // Assert
            var statusCodeResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Customer not found", statusCodeResult.Value);
        }
//Positive testcase to update the customer details based on the customer id
        [Fact]
        public async Task UpdateCustomerDetails_ValidCustomerDetails_ReturnsOk()
        {
            // Arrange
            int id = 1; // Assuming this ID exists in the database
            var customerDetails = new CustomerModel
            {
                Customer_FName = "John",
                Customer_LName = "Doe",
                Customer_Gender = "Male",
                Customer_Email = "john.doe@example.com",
                Customer_Mobile = "1234567890",
                SelectedAreas = new List<int> { 1, 2 }
            };

            var result = await _controller.UpdateCustomerDetails(id, customerDetails);

            // Assert: Verify the returned result
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Customer details updated successfully.", okResult.Value);

        }

//negative Testcase if the customer details is null value
        [Fact]
        public async Task UpdateCustomerDetails_NullCustomerDetails_ReturnsBadRequest()
        {
            // Arrange
            int customerId = 1; // Any valid customer ID

            // Act
            var result = await _controller.UpdateCustomerDetails(customerId, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Customer details are required.", badRequestResult.Value);
        }
//Negative Testcase for  the internal server 

        [Fact]
        public async Task UpdateCustomerDetails_NonExistingCustomer_ReturnsInternalServerError()
        {
            // Arrange
            int id = 9999; 
            var customerDetails = new CustomerModel
            {
                Customer_FName = "John12",
                Customer_LName = "Doe12",
                Customer_Gender = "Male",
                Customer_Email = "john@example.com",
                Customer_Mobile = "1234567890",
                SelectedAreas = new List<int> { 1, 2 }
            };

            // Act
            var result = await _controller.UpdateCustomerDetails(id, customerDetails);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error: Failed to update customer details.", statusCodeResult.Value);
        }
//Positive TestCase to deactivate the customer details
        [Fact]
        public async Task DeleteCustomerById_ValidCustomerId_ReturnsOk()
        {
            // Arrange

            int id = 6;

            // Act
            var result = await _controller.DeleteCustomerById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Customer deactivated successfully.", okResult.Value);

        }
//Negative TestCase id the id is zero and negative
        [Fact]
        public async Task DeleteCustomerById_InvalidCustomerId_ReturnsBadRequest()
        {
            // Arrange
            int id = -1;

            // Act
            var result = await _controller.DeleteCustomerById(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to find the customer details", badRequestResult.Value);
        }

//Negative TestCase if the customer is not deactivated

        [Fact]
        public async Task DeleteCustomerById_NonExistingCustomerId_ReturnsNotFound()
        {
            // Arrange
            int nonExistingCustomerId = 10000; 

            // Act
            var result = await _controller.DeleteCustomerById(nonExistingCustomerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Customer not found.", notFoundResult.Value);
        }

//Positive Test Case: Successfully Retrieving Customers
        [Fact]
        public async Task GetAllCustomer_Name_ReturnsOk()
        {
            // Act
            var result = _controller.GetAllCustomer_Name();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var customers = Assert.IsAssignableFrom<List<CustomerModel>>(okResult.Value);
            Assert.NotEmpty(customers);
  
        }



//positive Testcase if the customer name is already exists
        [Fact]
        public void IsCustomerFNameAvailable_Existing_ReturnsTrue()
        {
            // Arrange
            var request = new CustomerController.CustomerNameRequest { Customer_FName = "Surya Kumar" };

            // Act
            var result = _controller.IsCustomer_FNameAvailable(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            // Use reflection to access the Exists property
            var existsProperty = response.GetType().GetProperty("Exists");
            Assert.NotNull(existsProperty); // Ensure the property exists
            var existsValue = (bool)existsProperty.GetValue(response);
            Assert.True(existsValue);

        }
//Negative Testcase if the customer name is new

        [Fact]
        public void IsCustomerFNameAvailable_NonExisting_ReturnsFalse()
        {
            // Arrange
            var request = new CustomerController.CustomerNameRequest { Customer_FName = "ArjunReddyKaro" };

            // Act
            var result = _controller.IsCustomer_FNameAvailable(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value;
            // Use reflection to access the Exists property
            var existsProperty = response.GetType().GetProperty("Exists");
            Assert.NotNull(existsProperty); // Ensure the property exists
            var existsValue = (bool)existsProperty.GetValue(response);
            Assert.False(existsValue);


        }

//positive Testcase if the customer email is already exists
        [Fact]
        public void IsCustomerEmailAvailable_Existing_ReturnsTrue()
        {
            // Arrange
            var request = new CustomerController.EmailRequest { Email = "surya@gmail.com" };

            // Act
            var result = _controller.IsEmailAvailable(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            // Use reflection to access the Exists property
            var existsProperty = response.GetType().GetProperty("Exists");
            Assert.NotNull(existsProperty); // Ensure the property exists
            var existsValue = (bool)existsProperty.GetValue(response);
            Assert.True(existsValue);

        }

//Negative Testcase if the customer email is new

        [Fact]
        public void IsCustomerEmailAvailable_NonExisting_ReturnsFalse()
        {
            // Arrange
            var request = new CustomerController.EmailRequest { Email = "surya.thapovan@gmail.com" };

            // Act
            var result = _controller.IsEmailAvailable(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value;
            // Use reflection to access the Exists property
            var existsProperty = response.GetType().GetProperty("Exists");
            Assert.NotNull(existsProperty); // Ensure the property exists
            var existsValue = (bool)existsProperty.GetValue(response);
            Assert.False(existsValue);


        }

//Positive Testcase if the customer mobile number is already exists
        [Fact]
        public void IsCustomerMobileAvailable_Existing_ReturnsTrue()
        {
            // Arrange
            var request = new CustomerController.MobileRequest { Mobile = "6382421795" };

            // Act
            var result = _controller.IsMobileAvailable(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            // Use reflection to access the Exists property
            var existsProperty = response.GetType().GetProperty("Exists");
            Assert.NotNull(existsProperty); // Ensure the property exists
            var existsValue = (bool)existsProperty.GetValue(response);
            Assert.True(existsValue);

        }

//Negative Testcase if the customer mobile number is new

        [Fact]
        public void IsCustomerMobileAvailable_NonExisting_ReturnsFalse()
        {
            // Arrange
            var request = new CustomerController.MobileRequest { Mobile = "6382421794" };

            // Act
            var result = _controller.IsMobileAvailable(request);

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
