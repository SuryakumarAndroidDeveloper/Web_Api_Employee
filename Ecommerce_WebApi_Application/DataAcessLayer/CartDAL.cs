using MyCaRt.Models;
using System.Data.SqlClient;
using System.Data;
using Ecommerce_WebApi_Application.Models;

namespace Ecommerce_WebApi_Application.DataAcessLayer
{
    public class CartDAL
    {
        private readonly IConfiguration _configuration;

        public CartDAL(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        //Adding product to the Cart method

        public async Task<bool> AddToCart(CartItemModel cartItem)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("AddToCart", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        // cmd.Parameters.AddWithValue("@CartItem_Id", cartItem.CartItem_Id);
                        cmd.Parameters.AddWithValue("@Product_Id", cartItem.Product_Id);

                        cmd.Parameters.AddWithValue("@Customer_FName", cartItem.Customer_FName);

                        cmd.Parameters.AddWithValue("@Quantity", cartItem.Quantity);



                        connection.Open();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        //Get the cartitem based on customerid

        public List<DisplayCartModel> GetCartByCustomerId(int customerId)
        {
            List<DisplayCartModel> cartItems = new List<DisplayCartModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("GetCartByCustomerId", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Customer_FName", customerId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DisplayCartModel item = new DisplayCartModel
                    {
                        CartItem_Id = Convert.ToInt32(reader["CartItem_Id"]),
                        Product_Id = Convert.ToInt32(reader["Product_Id"]),
                        Product_Name = reader["Product_Name"].ToString(),
                        //Customer_FName = reader["Customer_FName"].ToString(),
                        Available_Quantity = Convert.ToInt32(reader["Available_Quantity"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Product_Price = Convert.ToDecimal(reader["Product_Price"])

                    };

                    cartItems.Add(item);
                }

                reader.Close();
            }

            return cartItems;
        }

        //UpdateCartItemQuantity based on customerid

        public bool UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("UpdateCartItemQuantity", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CartItem_Id", cartItemId);
                command.Parameters.AddWithValue("@NewQuantity", newQuantity);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        //Delete CArtItem based on customerid

        public bool DeleteCartItem(int cartItemId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("DeleteCartItem", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CartItem_Id", cartItemId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }





    }
}
