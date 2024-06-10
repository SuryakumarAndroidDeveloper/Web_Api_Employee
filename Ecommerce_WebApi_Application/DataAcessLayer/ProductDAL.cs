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

        public async Task<bool> InsertCustomerDetailsAsync(CustomerModel customerDetails)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertCustomer_Details", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@Customer_FName", customerDetails.Customer_FName);
                        cmd.Parameters.AddWithValue("@Customer_LName", customerDetails.Customer_LName);
                        cmd.Parameters.AddWithValue("@Customer_Gender", customerDetails.Customer_Gender);
                        cmd.Parameters.AddWithValue("@Customer_Email", customerDetails.Customer_Email);
                        cmd.Parameters.AddWithValue("@Customer_Mobile", customerDetails.Customer_Mobile);

                        // Create DataTable for TVP
                        DataTable interestedCategoryTable = new DataTable();
                        interestedCategoryTable.Columns.Add("Category_Id", typeof(int));

                        foreach (var category in customerDetails.SelectedAreas)
                        {
                            interestedCategoryTable.Rows.Add(category);
                        }

                        SqlParameter tvpParam = new SqlParameter
                        {
                            ParameterName = "@Customer_InterestedCategory",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.InterestedCategoryTableType",
                            Value = interestedCategoryTable
                        };

                        cmd.Parameters.Add(tvpParam);

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

        public async Task<List<CustomerInterestedCategory>> GetAllCustomersAsync()
        {
            List<CustomerInterestedCategory> customers = new List<CustomerInterestedCategory>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAllCustomer", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            int customer_Id = Convert.ToInt32(reader["Customer_Id"]);
                            CustomerInterestedCategory customer = new CustomerInterestedCategory
                            {
                                Customer_Id = customer_Id,
                                Customer_FName = reader["Customer_FName"].ToString(),
                                Customer_LName = reader["Customer_LName"].ToString(),
                                Customer_Gender = reader["Customer_Gender"].ToString(),
                                Customer_InterestedCategory = reader["Customer_InterestedCategory"].ToString(),
                                Customer_Email = reader["Customer_Email"].ToString(),
                                Customer_Mobile = reader["Customer_Mobile"].ToString(),
                            };
                            customers.Add(customer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return customers;
        }


        public async Task<CustomerInterestedCategory> GetCustomerByIdAsync(int id)
        {
            CustomerInterestedCategory customerModel = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetCustomerById", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Customer_Id", id);

                        connection.OpenAsync();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        if (await reader.ReadAsync())
                        {
                            customerModel = new CustomerInterestedCategory
                            {
                                Customer_Id = Convert.ToInt32(reader["Customer_Id"]),
                                Customer_FName = reader["Customer_FName"].ToString(),
                                Customer_LName = reader["Customer_LName"].ToString(),
                                Customer_Gender = reader["Customer_Gender"].ToString(),
                                Customer_InterestedCategory = reader["Customer_InterestedCategory"].ToString(),
                                Customer_Email = reader["Customer_Email"].ToString(),
                                Customer_Mobile = reader["Customer_Mobile"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return customerModel;
        }

        public async Task<bool> UpdateCustomerDetailsAsync(int id, CustomerModel customerDetails)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateCustomerDetails", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@Customer_Id", id);
                        cmd.Parameters.AddWithValue("@Customer_FName", customerDetails.Customer_FName);
                        cmd.Parameters.AddWithValue("@Customer_LName", customerDetails.Customer_LName);
                        cmd.Parameters.AddWithValue("@Customer_Gender", customerDetails.Customer_Gender);
                        cmd.Parameters.AddWithValue("@Customer_Email", customerDetails.Customer_Email);
                        cmd.Parameters.AddWithValue("@Customer_Mobile", customerDetails.Customer_Mobile);

                        // Create DataTable for TVP
                        DataTable interestedCategoryTable = new DataTable();
                        interestedCategoryTable.Columns.Add("Category_Id", typeof(int));

                        foreach (var category in customerDetails.SelectedAreas)
                        {
                            interestedCategoryTable.Rows.Add(category);
                        }

                        SqlParameter tvpParam = new SqlParameter
                        {
                            ParameterName = "@Customer_InterestedCategory",
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "dbo.InterestedCategoryTableType",
                            Value = interestedCategoryTable
                        };

                        cmd.Parameters.Add(tvpParam);

                        connection.Open();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (for example, using a logging framework)
                // Example: _logger.LogError(ex, "Error updating customer details.");
                return false;
            }
        }

        public async Task<bool> DeactivateCustomerByIdAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("DeactivateCustomer", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Customer_Id", id);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
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
                                Product_Price = Convert.ToInt32(reader["Product_Price"]),
                                Product_Description = reader["Product_Description"].ToString(),
                                Available_Quantity = Convert.ToInt32(reader["Available_Quantity"]),
                            };
                            products.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return products;
        }


        public List<CustomerModel> GetAllCustomer_Name()
        {
            List<CustomerModel> customer_Name = new List<CustomerModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("GetCustomer_Name", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CustomerModel customer = new CustomerModel
                        {
                            Customer_Id = Convert.ToInt32(reader["Customer_Id"]),
                            Customer_FName = reader["Customer_FName"].ToString(),

                        };
                        customer_Name.Add(customer);
                    }
                }
            }

            return customer_Name;
        }

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
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Product_Price = Convert.ToInt32(reader["Product_Price"])

                    };

                    cartItems.Add(item);
                }

                reader.Close();
            }

            return cartItems;
        }


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



        /*        public async Task<bool> PlaceOrder(int customerId,int productId,int quantity, decimal totalPrice)
                {
                    *//*         var dt = new DataTable();
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
                             }*//*
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                        {
                            await connection.OpenAsync();
                            using (SqlCommand cmd = new SqlCommand("Orderplaced", connection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                                cmd.Parameters.AddWithValue("@ProductId", productId);
                                cmd.Parameters.AddWithValue("@Quantity", quantity);
                                cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);

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
                }*/


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

   /* public interface IProductDAL
    {
        Task<bool> PlaceOrder(int customerId, int productId, int quantity, decimal totalPrice);
    }
*/










}
