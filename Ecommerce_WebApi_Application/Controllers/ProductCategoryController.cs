using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Http.HttpResults;

using System.Collections.Generic;
using System.Data.SqlClient;

namespace Ecommerce_WebApi_Application.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {

      
        public readonly ProductCategoryDAL _productcategoryDAL;
       

        public ProductCategoryController(IConfiguration configuration,ProductCategoryDAL productCategoryDAL)
        {
          
            _productcategoryDAL = productCategoryDAL;
        }

   

        [HttpPost]
        [Route("InsertProductCategory")]
        public IActionResult InsertProductCategory(ProductCategoryModel productCategory)
        {
            if (productCategory == null || string.IsNullOrWhiteSpace(productCategory.Category_Name))
            {
                return BadRequest("Failed to insert ProductCategory.");
            }
            if (_productcategoryDAL.IsCategory_NameAvailable(productCategory.Category_Name))
            {
                return BadRequest("Failed to insert ProductCategory.");
            }
            else
            {
                bool isInserted = _productcategoryDAL.InsertProductCategory(productCategory);

                if (isInserted)
                {
                    return Ok("ProductCategory inserted successfully.");
                }
                else
                {
                    return BadRequest("Failed to insert ProductCategory.");
                }
             
            }
        }



        [HttpGet]
        [Route("GetProductCategory")]
        public IActionResult GetProductCategory()
        {
            try
            {
                List<ProductCategoryModel> productCategories = _productcategoryDAL.GetProductCategory();

                if (productCategories.Count > 0)
                {
                    return Ok(productCategories);
                }
                else
                {
                    return NotFound("No product categories found.");
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

        [HttpPost]
        [Route("IsCategory_NameAvailable")]
        public IActionResult IsCategory_NameAvailable([FromBody] CategoryNameRequest request)
        {
            bool isAvailable = _productcategoryDAL.IsCategory_NameAvailable(request.CategoryName);

            if (isAvailable)
            {
                return Ok(new { Exists = isAvailable });
            }
            else
            {
                return BadRequest(new { Exists = isAvailable });
            }
        }

   

        public class CategoryNameRequest
        {
            public string CategoryName { get; set; }
        }


    }
}
