using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Ecommerce_WebApi_Application.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly ProductDAL _productDAL;

        public ProductController(IConfiguration configuration,ProductDAL productDAL)
        {


            _productDAL = productDAL;
        }


//Insert the product details api

        [HttpPost]
        [Route("InsertProduct_Details")]
        public IActionResult InsertProduct_Details(ProductModel product)
        {
            if (product == null || 
                string.IsNullOrWhiteSpace(product.Product_Category) ||
                string.IsNullOrWhiteSpace(product.Product_Code) ||
                string.IsNullOrWhiteSpace(product.Product_Name) ||
                string.IsNullOrWhiteSpace(product.Product_Price.ToString())||
                string.IsNullOrWhiteSpace(product.Available_Quantity.ToString())||
                string.IsNullOrWhiteSpace(product.Product_Description))
            {
                return BadRequest("Failed to Add Product.");
            }
            if (product.Product_Price < 0 || product.Available_Quantity < 0)
            {
                return BadRequest("Failed to Add Product.");
            }
            if (_productDAL.IsProduct_CodeAvailable(product.Product_Code))
            {
                return BadRequest("Failed to Add Product.");
            }
            bool isInserted = _productDAL.InsertProduct_Details(product);

            if (isInserted)
            {
                return Ok("Product Added successfully.");
            }
            else
            {
                return BadRequest("Failed to Add Product.");
            }
        }


 //get all the product api

        [HttpGet]
        [Route("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct()
        {
            try
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
            catch (SqlException ex)
            {
                // Log the exception (you can use any logging framework or mechanism you prefer)
                Console.WriteLine($"SQL Error: {ex.Message}");
                // Return a 500 Internal Server Error with the SQL error message
                return StatusCode(500, $"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception (you can use any logging framework or mechanism you prefer)
                Console.WriteLine($"Error: {ex.Message}");
                // Return a 500 Internal Server Error with the general error message
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        //validate product code

        [HttpPost]
        [Route("IsProduct_CodeAvailable")]
        public IActionResult IsProduct_CodeAvailable([FromBody] ProductCodeRequest request)
        {
            bool isAvailable = _productDAL.IsProduct_CodeAvailable(request.ProductCode);

            if (isAvailable)
            {
                return Ok(new { Exists = isAvailable });
            }
            else
            {
                return BadRequest(new { Exists = isAvailable });
            }
        }

        public class ProductCodeRequest
        {
            public string ProductCode { get; set; }
        }









    }
}
