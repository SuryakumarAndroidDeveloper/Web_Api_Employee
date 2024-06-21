using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;

namespace Ecommerce_WebApi_Application.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly WishListDAL _wishlistDAL;
        public WishListController(IConfiguration configuration,WishListDAL wishListDAL)
        {
            _wishlistDAL = wishListDAL; 

        }

        //add the product to wishlist based on customerid
        [HttpPost]
        [Route("AddToWishList")]
        public async Task<IActionResult> AddToWishList([FromBody] CartItemModel cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("cartItem are required.");
            }

            bool isSuccess = await _wishlistDAL.AddToWishList(cartItem);

            if (isSuccess)
            {
                return Ok("cartItem updated successfully.");
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to update  cartItem.");
            }
        }
        //get the wishlist based on customerid

        [HttpGet]
        [Route("GetWishListByCustomer")]
        public IActionResult GetWishListByCustomer(int customerId)
        {
            if (customerId == 0 || customerId < 0)
            {
                return BadRequest("CustomerId is NotFound");
            }
            List<MyWishlistModel> customerOrder = _wishlistDAL.GetWishListByCustomer(customerId);

            if (customerOrder.Count > 0)
            {

                return Ok(JsonConvert.SerializeObject(customerOrder));

            }
            else
            {
                return NotFound("No Wishlist found.");
            }

        }






    }
}
