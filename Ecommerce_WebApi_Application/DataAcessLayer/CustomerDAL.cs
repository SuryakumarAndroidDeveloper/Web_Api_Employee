using Ecommerce_WebApi_Application.Models;
using System.Data.SqlClient;
using System.Data;

namespace Ecommerce_WebApi_Application.DataAcessLayer
{
    public class CustomerDAL
    {
        public readonly IConfiguration _configuration;

        public CustomerDAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //Insert the CustomerDetails to DB
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

        //GetAllCustomer DAL Method from DB

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

        //GetCustomerByID method

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

        //UpdateCustomerDetailsByID method

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

        //DeleteCustomerByID method

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

        //GetAllCustomer_FName


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







    }
}
