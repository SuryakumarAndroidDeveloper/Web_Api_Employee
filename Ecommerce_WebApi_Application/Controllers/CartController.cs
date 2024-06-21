using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;

namespace Ecommerce_WebApi_Application.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly CartDAL _cartDAL;

        public CartController(IConfiguration configuration,CartDAL cartDAL)
        {
           _cartDAL = cartDAL;

        }

        //Add the porduct to the cart based on customerid

        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemModel cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("cartItem are required.");
            }
            if(cartItem.Product_Id <0) {
                return BadRequest("Add cartItem Failed.");
            }

            bool isSuccess = await _cartDAL.AddToCart(cartItem);

            if (isSuccess)
            {
                return Ok("cartItem updated successfully.");
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to update cartItem.");
            }
        }
        //get the cartitems based on customerid
        [HttpGet]
        [Route("GetCartByCustomer")]
        public IActionResult GetCartByCustomer(int customerId)
        {
            if(customerId <= 0)
            {
                return BadRequest("Failed to load Cart.");
            }
            List<DisplayCartModel> customerCart = _cartDAL.GetCartByCustomerId(customerId);

            if (customerCart.Count > 0)
            {

                return Ok(customerCart);

            }
            else
            {
                return NotFound("No cart found.");
            }

        }
        //update the quantity in cartitem

        [HttpPost]
        [Route("UpdateCartItemQuantity")]
        public IActionResult UpdateCartItemQuantity([FromBody] UpdateCartItemQuantityModel model)
        {
            if (model==null) {
                return BadRequest("Failed");
            }
            if (model.NewQuantity < 0)
            {
                return BadRequest("Failed to update quantity.");
            }
            bool isUpdated = _cartDAL.UpdateCartItemQuantity(model.CartItemId, model.NewQuantity);

            if (isUpdated)
            {
                return Ok("Quantity updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update quantity.");
            }
        }
        //delete the cartitem based on customerid

        [HttpPost]
        [Route("DeleteCartItem")]
        public IActionResult DeleteCartItem(int cartItemId)
        {
            if (cartItemId <= 0)
            {
                return BadRequest("Failed to found item.");
            }
            bool isDeleted = _cartDAL.DeleteCartItem(cartItemId);

            if (isDeleted)
            {
                return Ok("Item deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete item.");
            }
        }


    }
}
