using MyCaRt.Models;
using System.Data.SqlClient;
using System.Data;
using Ecommerce_WebApi_Application.Models;

namespace Ecommerce_WebApi_Application.DataAcessLayer
{
    public class WishListDAL
    {
        private readonly IConfiguration _configuration;

        public WishListDAL(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        //Add the product to wishlist based on the customer_FName method

        public async Task<bool> AddToWishList(CartItemModel cartItem)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("AddToWishList", connection))
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

        //get the wishlist based on customerid

        public List<MyWishlistModel> GetWishListByCustomer(int customerId)
        {
            List<MyWishlistModel> myWishlists = new List<MyWishlistModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("GetWishListByCustomerId", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Customer_Id", customerId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MyWishlistModel item = new MyWishlistModel
                    {
                        Customer_Id = Convert.ToInt32(reader["Customer_Id"]),
                        Product_Id = Convert.ToInt32(reader["Product_Id"]),
                        Customer_FName = reader["Customer_FName"].ToString(),
                        Product_Category = reader["Category_Name"].ToString(),
                        Product_Code = reader["Product_Code"].ToString(),
                        Product_Name = reader["Product_Name"].ToString(),
                        Product_Price = Convert.ToInt32(reader["Product_Price"]),
                        Product_Description = reader["Product_Description"].ToString(),
                        Available_Quantity = Convert.ToInt32(reader["Available_Quantity"])
                    };

                    myWishlists.Add(item);
                }

                reader.Close();
            }

            return myWishlists;
        }





    }
}
