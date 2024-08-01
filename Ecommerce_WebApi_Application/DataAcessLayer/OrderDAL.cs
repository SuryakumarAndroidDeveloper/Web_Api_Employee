using Ecommerce_WebApi_Application.Models;
using System.Data.SqlClient;
using System.Data;

namespace Ecommerce_WebApi_Application.DataAcessLayer
{
    public class OrderDAL
    {
        private readonly IConfiguration _configuration;

        public OrderDAL(IConfiguration configuration)
        {
            _configuration = configuration;

        }

 //placeorder based on customerid

        public async Task<bool> PlaceOrder(int customerId,int paymentId ,List<OrderProduct> orderProducts)
        {
            var dt = new DataTable();
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("TotalPrice", typeof(decimal));

            foreach (var product in orderProducts)
            {
                dt.Rows.Add(product.Product_Id, product.Quantity, product.TotalPrice);
            }

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();
                    using (var cmd = new SqlCommand("Orderplaced", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CustomerId", customerId);
                        cmd.Parameters.AddWithValue("@PaymentId", paymentId);
                        var tableParam = cmd.Parameters.AddWithValue("@Products", dt);
                        tableParam.SqlDbType = SqlDbType.Structured;
                        tableParam.TypeName = "dbo.OrderProductType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true; // Return true if order placed successfully
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your application's error handling strategy
                return false; // Return false if failed to place the order
            }
        }

        //Get the orders based on customerid
        public List<MyOrderModel> GetOrderByCustomerId(int customerId)
        {
            List<MyOrderModel> myOrders = new List<MyOrderModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("GetOrderByCustomerId", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Customer_Id", customerId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MyOrderModel item = new MyOrderModel
                    {
                        OrderId = Convert.ToInt32(reader["OrderId"]),
                        Customer_FName = reader["Customer_FName"] != DBNull.Value ? reader["Customer_FName"].ToString() : null,
                        Product_Name = reader["Product_Name"] != DBNull.Value ? reader["Product_Name"].ToString() : null,
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        TotalPrice = reader["TotalPrice"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPrice"]) : null,
                        OrderDate = reader["OrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["OrderDate"]) : null,
                        IsPaid = reader["IsPaid"].ToString(),
                        DeliveryDate = reader["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(reader["DeliveryDate"]) : (DateTime?)null,
                        DeliveryStatus = reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : null,
                    };

                    myOrders.Add(item);
                }

                reader.Close();
            }

            return myOrders;
        }


        //CancelOrderByID method

        public async Task<bool> CancelOrderByIdAsync(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("CancelOrder", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Order_Id", orderId);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }



        //GetAllOrders DAL Method from DB

        public async Task<List<ListOfOrdersModel>> GetAllOrdersAsync()
        {
            List<ListOfOrdersModel> orders = new List<ListOfOrdersModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAllOrders", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            ListOfOrdersModel order = new ListOfOrdersModel
                            {
                                //OrderDetailId = Convert.ToInt32(reader["OrderDetailId"]),
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                TotalPrice = reader["TotalPrice"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPrice"]) : 0,
                                OrderDate = reader["OrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["OrderDate"]) : (DateTime?)null,
                                IsPaid = reader["IsPaid"] != DBNull.Value ? reader["IsPaid"].ToString() : null,
                                DeliveryDate = reader["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(reader["DeliveryDate"]) : (DateTime?)null,
                                DeliveryStatus = reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : null,
                            };
                            orders.Add(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return orders;
        }

        //get the orderdetails based on the orderId

        public async Task<ListOfOrdersModel> GetOrderByIdAsync(int orderId)
        {
            ListOfOrdersModel orderModel = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetOrderDetailsByOrderId", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderId", orderId);

                        connection.Open();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (await reader.ReadAsync())
                        {
                            orderModel = new ListOfOrdersModel
                            {
                               // OrderDetailId = Convert.ToInt32(reader["OrderDetailId"]),
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                TotalPrice = reader["TotalPrice"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPrice"]) : 0,
                                OrderDate = reader["OrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["OrderDate"]) : (DateTime?)null,
                                IsPaid = reader["IsPaid"] != DBNull.Value ? reader["IsPaid"].ToString() : null,
                                DeliveryDate = reader["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(reader["DeliveryDate"]) : (DateTime?)null,
                                DeliveryStatus = reader["DeliveryStatus"] != DBNull.Value ? reader["DeliveryStatus"].ToString() : null,
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return orderModel;
        }

        //post the upodated order details based on the orderId


        public async Task<bool> UpdateOrderDetailsAsync(int orderId, ListOfOrdersModel orderDetails)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateOrderDetailsByOrderID", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@ProductId", orderDetails.ProductId);
                        cmd.Parameters.AddWithValue("@Quantity", orderDetails.Quantity);
                        cmd.Parameters.AddWithValue("@TotalPrice", orderDetails.TotalPrice);
                        cmd.Parameters.AddWithValue("@OrderDate", orderDetails.OrderDate);
                        cmd.Parameters.AddWithValue("@IsPaid", orderDetails.IsPaid);
                        cmd.Parameters.AddWithValue("@DeliveryDate", orderDetails.DeliveryDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DeliveryStatus", orderDetails.DeliveryStatus ?? (object)DBNull.Value);

                        connection.Open();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }
}
