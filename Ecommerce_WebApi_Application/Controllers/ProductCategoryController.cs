using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecommerce_WebApi_Application.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {

        public readonly IConfiguration _configuration;
        public readonly ProductCategoryDAL _productcategoryDAL;

        public ProductCategoryController(IConfiguration configuration)
        {
            _configuration = configuration;

            _productcategoryDAL = new ProductCategoryDAL(configuration);

        }



        [HttpPost]
        [Route("InsertProductCategory")]
        public IActionResult InsertProductCategory(ProductCategoryModel productCategory)
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



        [HttpGet]
        [Route("GetProductCategory")]
        public String GetProductCategory()
        {
            List<ProductCategoryModel> productCategories = _productcategoryDAL.GetProductCategory();

            if (productCategories.Count > 0)
            {

                return JsonConvert.SerializeObject(productCategories);

            }
            else
            {
                return "No ProductCategory found.";
            }

        }


    }
}
