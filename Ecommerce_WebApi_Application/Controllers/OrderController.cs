using Ecommerce_WebApi_Application.DataAcessLayer;
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

        public OrderController(IConfiguration configuration,OrderDAL orderDAL)
        {
            _orderDAL = orderDAL;

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

                // Collect all order products into a single list
                var allOrderProducts = new List<OrderProduct>();
                foreach (var orderProduct in orderRequest.OrderProducts)
                {
                    allOrderProducts.Add(orderProduct);
                }

                // Call the DAL method to place the order
                bool isOrderPlaced = await _orderDAL.PlaceOrder(orderRequest.Customer_Id,orderRequest.PaymentId, allOrderProducts);

                if (isOrderPlaced)
                {
                    return Ok("Order placed successfully.");
                }
                else
                {
                    return BadRequest("Failed to place the order.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your application's error handling strategy
                return BadRequest("Failed to place the order.");
            }
        }
        //BuyAgainOrder
        [HttpPost]
        [Route("BuyAgainOrder")]
        public async Task<IActionResult> BuyAgainOrder([FromBody] BuyAgainOrderModel buyAgainOrder)
        {
            bool isOrderPlaced = await _orderDAL.BuyAgainOrder(buyAgainOrder.PaymentId.Value,buyAgainOrder.OrderId.Value);

            if (isOrderPlaced)
            {
                return Ok("Order placed successfully.");
            }
            else
            {
                return BadRequest("Failed to place the order.");
            }
        }

        [HttpPost]
        [Route("UpdateFullOrderDetails")]
        public async Task<IActionResult> UpdateFullOrderDetails([FromBody] List<ListOfOrdersModel> orders)
        {
            if (orders == null || !orders.Any())
            {
                return BadRequest("No orders to update.");
            }

            try
            {
                bool isUpdated = await _orderDAL.UpdateOrdersAsync(orders);

                if (isUpdated)
                {
                    return Ok("Orders updated successfully.");
                }
                else
                {
                    return StatusCode(500, "An error occurred while updating the orders.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /*       [HttpPost]
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
               }*/
        //get the orders based on the customerid

        [HttpGet]
        [Route("GetOrderByCustomer")]
        public IActionResult GetOrderByCustomer(int customerId)
        {
            if (customerId==0 )
            {
                return BadRequest("Failed to load order.");
            }
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

        //delete the customer details based on customerid (softdelete)

        [HttpPost]
        [Route("CancelOrderById")]
        public async Task<IActionResult> CancelOrderById(int orderId)
        {
            if (orderId < 0 || orderId == 0)
            {
                return BadRequest("Failed to find the order");
            }
            try
            {
                bool isCanceled = await _orderDAL.CancelOrderByIdAsync(orderId);

                if (isCanceled)
                {
                    return Ok("Canceled successfully.");
                }
                else
                {
                    return NotFound("Order not found.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal server error");
            }
        }
        //GetAllorders method

        [HttpGet]
        [Route("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            List<ListOfOrdersModel> orders = await _orderDAL.GetAllOrdersAsync();

            if (orders != null && orders.Count > 0)
            {
                return Ok(orders);
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to get customer details.");
            }
        }


        //gettheorder details based on the orderid

        [HttpGet]
        [Route("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            if (orderId < 0)
            {
                return BadRequest("Fail to get Customer");
            }
            var order = await _orderDAL.GetOrderByIdAsync(orderId);

            if (order != null)
            {
                return Ok(order);
            }
            else
            {
                return NotFound("Customer not found");
            }
        }


        //update the deliverydate and deliverystatus based on the orderid

        [HttpPost]
        [Route("UpdateOrderDetails")]
        public async Task<IActionResult> UpdateOrderDetails(int orderId, [FromBody] ListOfOrdersModel orderDetails)
        {
            if (orderDetails == null)
            {
                return BadRequest("Customer details are required.");
            }

            bool isSuccess = await _orderDAL.UpdateOrderDetailsAsync(orderId, orderDetails);

            if (isSuccess)
            {
                return Ok("Customer details updated successfully.");
            }
            else
            {
                return StatusCode(500, "Internal server error: Failed to update customer details.");
            }
        }











    }
}
