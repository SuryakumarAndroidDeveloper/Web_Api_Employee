using Ecommerce_WebApi_Application.Models;
using System.Data.SqlClient;
using System.Data;


namespace Ecommerce_WebApi_Application.DataAcessLayer
{
    public class ProductCategoryDAL
    {
        public readonly IConfiguration _configuration;

        public ProductCategoryDAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }

//Insert the productCategory to db
        public bool InsertProductCategory(ProductCategoryModel productCategory, out string errorMessage)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("InsertProductCategory", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Category_Name", productCategory.Category_Name);


                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParameter.Value;

                    if (result == -1)
                    {
                        errorMessage = "Category name already exists.";
                        return false;
                    }

                    errorMessage = null;
                    return result == 0;
                }
            }
        }
        //Get the ProductCategory from DB
        public List<ProductCategoryModel> GetProductCategory()
        {
            List<ProductCategoryModel> productCategories = new List<ProductCategoryModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand command = new SqlCommand("GetProductCategory", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductCategoryModel product = new ProductCategoryModel
                            {
                                Category_Id = Convert.ToInt32(reader["Category_Id"]),
                                Category_Name = reader["Category_Name"].ToString(),
                            };
                            productCategories.Add(product);
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

            return productCategories;
        }


/*        public bool IsCategory_NameAvailable(string categoryName)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("CheckCategory_Name", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Category_Name", categoryName);

                    connection.Open();
                    int count = (int)cmd.ExecuteScalar(); // Use ExecuteScalar to get the count result

                    return count > 0; // Return true if the category name exists
                }
            }
        }*/






    }


}
