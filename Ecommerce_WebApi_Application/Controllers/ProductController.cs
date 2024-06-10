using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_WebApi_Application.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly ProductDAL _productDAL;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;

            _productDAL = new ProductDAL(configuration);
        }


//Insert the product details api

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


 //get all the product api

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










    }
}
