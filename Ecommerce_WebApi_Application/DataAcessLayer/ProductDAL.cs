using Ecommerce_WebApi_Application.Models;
using Microsoft.Extensions.Configuration;
using MyCaRt.Models;
using System.Data;
using System.Data.SqlClient;

namespace Ecommerce_WebApi_Application.DataAcessLayer
{
    public class ProductDAL 
    {
        public readonly IConfiguration _configuration;

        public ProductDAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }






        public bool InsertProduct_Details(ProductModel product)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("InsertProductDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Product_Category", product.Product_Category);
                    cmd.Parameters.AddWithValue("@Product_Code", product.Product_Code);
                    cmd.Parameters.AddWithValue("@Product_Name", product.Product_Name);
                    cmd.Parameters.AddWithValue("@Product_Price", product.Product_Price);
                    cmd.Parameters.AddWithValue("@Product_Description", product.Product_Description);
                    cmd.Parameters.AddWithValue("@Available_Quantity", product.Available_Quantity);


                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }


        public async Task<List<ProductModel>> GetAllProductAsync()
        {
            List<ProductModel> products = new List<ProductModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAllProduct", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {

                            ProductModel product = new ProductModel
                            {
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Product_Category = reader["Product_Category"].ToString(),
                                Product_Code = reader["Product_Code"].ToString(),
                                Product_Name = reader["Product_Name"].ToString(),
                                Product_Price = Convert.ToDecimal(reader["Product_Price"]),
                                Product_Description = reader["Product_Description"].ToString(),
                                Available_Quantity = Convert.ToInt32(reader["Available_Quantity"]),
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log the exception (you can use any logging framework or mechanism you prefer)
                Console.WriteLine($"SQL Error: {ex.Message}");
                // Rethrow the exception to be handled by the controller
                throw;
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return products;
        }




        public bool IsProduct_CodeAvailable(string productCode)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("CheckProduct_Code", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Product_Code", productCode);

                    connection.Open();
                    int count = (int)cmd.ExecuteScalar(); // Use ExecuteScalar to get the count result

                    return count > 0; // Return true if the category name exists
                }
            }
        }


    }

}
