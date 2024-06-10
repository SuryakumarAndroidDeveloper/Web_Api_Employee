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
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PaymentDAL paymentDAL, ILogger<PaymentController> logger)
        {
            _paymentDAL = paymentDAL;
            _logger = logger;
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
                var isStored = await _paymentDAL.StorePaymentData(paymentModel);

                if (isStored)
                {
                    return Ok("Payment stored successfully!");
                }
                else
                {
                    _logger.LogError("Failed to store the payment: Unknown reason.");
                    return BadRequest("Failed to store the payment.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to store the payment: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
