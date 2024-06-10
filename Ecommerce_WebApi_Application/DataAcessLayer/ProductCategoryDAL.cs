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
        public bool InsertProductCategory(ProductCategoryModel productCategory)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("InsertProductCategory", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Category_Name", productCategory.Category_Name);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }
//Get the ProductCategory from DB
        public List<ProductCategoryModel> GetProductCategory()
        {
            List<ProductCategoryModel> productCategories = new List<ProductCategoryModel>();

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

            return productCategories;
        }






    }
}
