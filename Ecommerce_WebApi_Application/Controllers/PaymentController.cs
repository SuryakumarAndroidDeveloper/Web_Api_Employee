using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ecommerce_WebApi_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentDAL _paymentDAL;
       

        public PaymentController(IConfiguration configuration,PaymentDAL paymentDAL)
        {
            _paymentDAL = paymentDAL;
            
        }

        [HttpPost]
        [Route("AddPayment")]
        public async Task<IActionResult> AddPayment([FromBody] PaymentModel paymentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payment data.");
            }
            try
            {
                var paymentId = await _paymentDAL.StorePaymentData(paymentModel);

                if (paymentId.HasValue)
                {
                    // return Ok("Payment stored successfully!");
                    return Ok(new { PaymentId = paymentId.Value });
                }
                else
                {
                 
                    return BadRequest("Failed to store the payment.");
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
