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






        public bool InsertProduct_Details(ProductModel product, out string errorMessage)
        {
            errorMessage = null;
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
                    cmd.Parameters.AddWithValue("@FilePath", product.FilePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImageName", product.ImageName ?? (object)DBNull.Value);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }

                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("Product code already exists."))
                        {
                            errorMessage = "Product code already exists.";
                        }
                        else
                        {
                            errorMessage = "An error occurred while inserting the product details.";
                        }
                        return false;
                    }
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
                                FilePath = reader["FilePath"].ToString(),
                                ImageName = reader["ImageName"].ToString(),
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

    }

}
