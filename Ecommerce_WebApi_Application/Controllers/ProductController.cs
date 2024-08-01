using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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



        [HttpPost("InsertProductJsonData")]
        public async Task<IActionResult> InsertProductJsonData([FromBody] IEnumerable<ProductModel> jsonData)
        {
            if (jsonData == null || !jsonData.Any())
            {
                return BadRequest("Invalid data.");
            }

            await _productDAL.InsertJsonDataToDatabase(jsonData);
            return Ok("Data inserted successfully");
        }




        //Insert the product details api

        [HttpPost]
        [Route("InsertProduct_Details")]
        public async Task<IActionResult> InsertProduct_Details([FromBody] List<ProductModel> products)
        {
            if (products == null || !products.Any())
            {
                return BadRequest("No products to add.");
            }
            List<string> errorMessages = new List<string>();
            List<ValidationError> errors = new List<ValidationError>();
            foreach (var product in products)
            {
               /* if (string.IsNullOrWhiteSpace(product.Product_Category) ||
                    string.IsNullOrWhiteSpace(product.Product_Code) ||
                    string.IsNullOrWhiteSpace(product.Product_Name) ||
                    string.IsNullOrWhiteSpace(product.Product_Price.ToString()) ||
                    string.IsNullOrWhiteSpace(product.Available_Quantity.ToString()) ||
                    string.IsNullOrWhiteSpace(product.Product_Description))
                {
                    return BadRequest("Failed to add product.");
                }*/
                if (product.Product_Price < 0 || product.Available_Quantity < 0)
                {
                    return BadRequest("Failed to add product.");
                }

                string errorMessage;
                // bool isInserted = _productDAL.InsertProduct_Details(product, out errorMessage);
                bool isProductCodeExists = _productDAL.IsProductCodeExists(product.Product_Code);
                if (isProductCodeExists)
                {
                    // return Ok(errorMessage);
                    errors.Add(new ValidationError
                    {
                        Field = $"[{products.IndexOf(product)}].Product_Code",
                        Error = "Product code already exists."
                    });
                }
            }
              if (errors.Any())
               {
                   return BadRequest(errors); // Return validation errors
               }
            foreach (var product in products)
            {
                // Insert each product into database
                string errorMessage;
                bool isInserted = _productDAL.InsertProduct_Details(product, out errorMessage);

                if (!isInserted)
                {
                    // Handle insertion failure if needed
                    return BadRequest("Failed to insert products into the database.");
                }
            }
            return Ok("Products added successfully.");
               
        }
             public class ValidationError
        {
            public string Field { get; set; }
            public string Error { get; set; }
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






    }
}
