using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ecommerce_WebApi_Application.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly CustomerDAL _customerDAL;

        public CustomerController(IConfiguration configuration,CustomerDAL customerDAL)
        {
            _customerDAL = customerDAL;

        }

        //Insert the customerdetails

        [HttpPost]
        [Route("InsertCustomerDetails")]
        public async Task<IActionResult> InsertCustomerDetails([FromBody] CustomerModel customerDetails)
        {
            bool isSuccess = await _customerDAL.InsertCustomerDetailsAsync(customerDetails);

            if (isSuccess)
            {
                return Ok("Customer details inserted successfully.");
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to insert customer details.");
            }
        }
        //GetAllcustomer method

        [HttpGet]
        [Route("GetAllCustomer")]
        public async Task<IActionResult> GetAllCustomer()
        {
            List<CustomerInterestedCategory> customers = await _customerDAL.GetAllCustomersAsync();

            if (customers != null && customers.Count > 0)
            {
                return Ok(customers);
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to get customer details.");
            }
        }
        //Get the Customer based on customerid
        [HttpGet]
        [Route("GetCustomerByID")]
        public async Task<IActionResult> GetCustomerByID(int id)
        {
            if (id < 0)
            {
                return BadRequest("Fail to get Customer");
            }
            var customer = await _customerDAL.GetCustomerByIdAsync(id);
          
            if (customer != null)
            {
                return Ok(customer);
            }
            else
            {
                return NotFound("Customer not found");
            }
        }
        //Update the customer details based on customerid
        [HttpPost]
        [Route("UpdateCustomerDetails")]
        public async Task<IActionResult> UpdateCustomerDetails(int id, [FromBody] CustomerModel customerDetails)
        {
            if (customerDetails == null)
            {
                return BadRequest("Customer details are required.");
            }

            bool isSuccess = await _customerDAL.UpdateCustomerDetailsAsync(id, customerDetails);

            if (isSuccess)
            {
                return Ok("Customer details updated successfully.");
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to update customer details.");
            }
        }
        //delete the customer details based on customerid (softdelete)

        [HttpPost]
        [Route("DeleteCustomerById")]
        public async Task<IActionResult> DeleteCustomerById(int id)
        {
            if (id<0 || id ==0)
            {
                return BadRequest("Failed to find the customer details");
            }
            try
            {
                bool isDeactivated = await _customerDAL.DeactivateCustomerByIdAsync(id);

                if (isDeactivated)
                {
                    return Ok("Customer deactivated successfully.");
                }
                else
                {
                    return NotFound("Customer not found.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }
        }

        //Getall the customer_FName


        [HttpGet]
        [Route("GetAllCustomer_Name")]
        public IActionResult GetAllCustomer_Name()
        {
            List<CustomerModel> customer_Name = _customerDAL.GetAllCustomer_Name();

            if (customer_Name.Count > 0)
            {

                return Ok(customer_Name);

            }
            else
            {
                return NotFound("Customer Not Found.");
            }

        }
        //validate customer name
        [HttpPost]
        [Route("IsCustomer_FNameAvailable")]
        public IActionResult IsCustomer_FNameAvailable([FromBody] CustomerNameRequest request)
        {
            bool isAvailable = _customerDAL.IsCustomer_FNameAvailable(request.Customer_FName);

            if (isAvailable)
            {
                return Ok(new { Exists = isAvailable });
            }
            else
            {
                return BadRequest(new { Exists = isAvailable });
            }
        }

        public class CustomerNameRequest
        {
            public string Customer_FName { get; set; }
        }
        //validate email
        [HttpPost]
        [Route("IsEmailAvailable")]
        public IActionResult IsEmailAvailable([FromBody] EmailRequest request)
        {
            bool isAvailable = _customerDAL.IsEmailAvailable(request.Email);

            if (isAvailable)
            {
                return Ok(new { Exists = isAvailable });
            }
            else
            {
                return BadRequest(new { Exists = isAvailable });
            }
        }

        public class EmailRequest
        {
            public string Email { get; set; }
        }
        //validate mobile number
        [HttpPost]
        [Route("IsMobileAvailable")]
        public IActionResult IsMobileAvailable([FromBody] MobileRequest request)
        {
            bool isAvailable = _customerDAL.IsMobileAvailable(request.Mobile);

            if (isAvailable)
            {
                return Ok(new { Exists = isAvailable });
            }
            else
            {
                return BadRequest(new { Exists = isAvailable });
            }
        }

        public class MobileRequest
        {
            public string Mobile { get; set; }
        }


    }
}
