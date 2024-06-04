using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;

namespace Ecommerce_WebApi_Application.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {

        public readonly IConfiguration _configuration;
        public readonly ProductDAL _productDAL;

        public ProductCategoryController(IConfiguration configuration)
        {
            _configuration = configuration;

            _productDAL = new ProductDAL(configuration);

        }



        [HttpPost]
        [Route("InsertProductCategory")]
        public IActionResult InsertProductCategory(ProductCategoryModel productCategory)
        {
            bool isInserted = _productDAL.InsertProductCategory(productCategory);

            if (isInserted)
            {
                return Ok("ProductCategory inserted successfully.");
            }
            else
            {
                return BadRequest("Failed to insert ProductCategory.");
            }
        }


        [HttpPost]
        [Route("InsertProduct_Details")]
        public IActionResult InsertProduct_Details(ProductModel product)
        {
            bool isInserted = _productDAL.InsertProduct_Details(product);

            if (isInserted)
            {
                return Ok("Product   Added successfully.");
            }
            else
            {
                return BadRequest("Failed to Add Product.");
            }
        }

        [HttpGet]
        [Route("GetProductCategory")]
        public String GetProductCategory()
        {
            List<ProductCategoryModel> productCategories = _productDAL.GetProductCategory();

            if (productCategories.Count > 0)
            {

                return JsonConvert.SerializeObject(productCategories);

            }
            else
            {
                return "No ProductCategory found.";
            }

        }


        [HttpPost]
        [Route("InsertCustomerDetails")]
        public async Task<IActionResult> InsertCustomerDetails([FromBody] CustomerModel customerDetails)
        {
            bool isSuccess = await _productDAL.InsertCustomerDetailsAsync(customerDetails);

            if (isSuccess)
            {
                return Ok("Customer details inserted successfully.");
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to insert customer details.");
            }
        }




        [HttpGet]
        [Route("GetAllCustomer")]
        public async Task<IActionResult> GetAllCustomer()
        {
            List<CustomerInterestedCategory> customers = await _productDAL.GetAllCustomersAsync();

            if (customers != null && customers.Count > 0)
            {
                return Ok(customers);
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to get customer details.");
            }
        }
        [HttpGet]
        [Route("GetCustomerByID")]
        public async Task<IActionResult> GetCustomerByID(int id)
        {
            var customer = await _productDAL.GetCustomerByIdAsync(id);

            if (customer != null)
            {
                return Ok(customer);
            }
            else
            {
                return NotFound("Customer not found");
            }
        }

        [HttpPost]
        [Route("UpdateCustomerDetails")]
        public async Task<IActionResult> UpdateCustomerDetails(int id, [FromBody] CustomerModel customerDetails)
        {
            if (customerDetails == null)
            {
                return BadRequest("Customer details are required.");
            }

            bool isSuccess = await _productDAL.UpdateCustomerDetailsAsync(id, customerDetails);

            if (isSuccess)
            {
                return Ok("Customer details updated successfully.");
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to update customer details.");
            }
        }


        [HttpPost]
        [Route("DeleteCustomerById")]
        public async Task<IActionResult> DeleteCustomerById(int id)
        {
            try
            {
                bool isDeactivated = await _productDAL.DeactivateCustomerByIdAsync(id);

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


        [HttpGet]
        [Route("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct()
        {
            List<ProductModel> products = await _productDAL.GetAllProductAsync();

            if (products != null && products.Count > 0)
            {
                return Ok(products);
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to get customer details.");
            }
        }


        [HttpGet]
        [Route("GetAllCustomer_Name")]
        public String GetAllCustomer_Name()
        {
            List<CustomerModel> customer_Name = _productDAL.GetAllCustomer_Name();

            if (customer_Name.Count > 0)
            {

                return JsonConvert.SerializeObject(customer_Name);

            }
            else
            {
                return "No ProductCategory found.";
            }

        }


        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemModel cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("cartItem are required.");
            }

            bool isSuccess = await _productDAL.AddToCart(cartItem);

            if (isSuccess)
            {
                return Ok("cartItem updated successfully.");
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to update  cartItem.");
            }
        }


















    }
}
