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

        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
            _customerDAL = new CustomerDAL(configuration);

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
        public String GetAllCustomer_Name()
        {
            List<CustomerModel> customer_Name = _customerDAL.GetAllCustomer_Name();

            if (customer_Name.Count > 0)
            {

                return JsonConvert.SerializeObject(customer_Name);

            }
            else
            {
                return "No ProductCategory found.";
            }

        }



    }
}
