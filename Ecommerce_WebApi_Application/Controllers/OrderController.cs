﻿using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ecommerce_WebApi_Application.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly OrderDAL _orderDAL;

        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
            _orderDAL = new OrderDAL(configuration);

        }

        //place order for the customer

        [HttpPost]
        [Route("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderProductModel orderRequest)
        {
            try
            {
                if (orderRequest == null || orderRequest.OrderProducts == null || orderRequest.OrderProducts.Count == 0)
                {
                    return BadRequest("Invalid order data.");
                }

                foreach (var orderProduct in orderRequest.OrderProducts)
                {
                    bool isOrderPlaced = await _orderDAL.PlaceOrder(orderRequest.Customer_Id, orderRequest.OrderProducts);

                    if (!isOrderPlaced)
                    {
                        return BadRequest("Failed to place the order.");
                    }
                }

                return Ok("Order placed successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your application's error handling strategy
                return BadRequest("Failed to place the orders.");
            }
        }
        //get the orders based on the customerid

        [HttpGet]
        [Route("GetOrderByCustomer")]
        public IActionResult GetOrderByCustomer(int customerId)
        {
            List<MyOrderModel> customerOrder = _orderDAL.GetOrderByCustomerId(customerId);

            if (customerOrder.Count > 0)
            {

                return Ok(JsonConvert.SerializeObject(customerOrder));

            }
            else
            {
                return NotFound("No order found.");
            }

        }





    }
}
