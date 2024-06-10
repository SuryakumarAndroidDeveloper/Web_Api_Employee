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

        public async Task<bool> PlaceOrder(int customerId, List<OrderProduct> orderProducts)
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
                        IsPaid = reader["IsPaid"].ToString()
                    };

                    myOrders.Add(item);
                }

                reader.Close();
            }

            return myOrders;
        }



    }
}
