using EmployeeManagement.Models;
using System.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.DataAcessLayer
{
    public class EmployeDAL
    {

        private readonly IConfiguration _configuration;

        public EmployeDAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public List<EmployeModel> GetActiveEmployees()
        {
            List<EmployeModel> activeEmployees = new List<EmployeModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("GetActiveEmployees_New", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EmployeModel employee = new EmployeModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CompanyName = reader["CompanyName"].ToString(),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Mobile = reader["Mobile"].ToString(),
                            Address = reader["Address"].ToString(),
                            City = reader["City"].ToString(),
                            Pincode = Convert.ToInt32(reader["Pincode"])
                        };
                        activeEmployees.Add(employee);
                    }
                }
            }

            return activeEmployees;
        }



        public bool InsertEmployee(EmployeModel employee)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("InsertEmployee_New", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CompanyName", employee.CompanyName);
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Mobile", employee.Mobile);
                    cmd.Parameters.AddWithValue("@Address", employee.Address);
                    cmd.Parameters.AddWithValue("@City", employee.City);
                    cmd.Parameters.AddWithValue("@Pincode", employee.Pincode);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }



        public async Task<EmployeModel> GetEmployeeByIdAsync(int id)
        {
            EmployeModel employeModel = null;

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("GetEmployById_New", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    await connection.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            employeModel = new EmployeModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CompanyName = reader["CompanyName"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = Convert.ToInt32(reader["Pincode"]),
                            };
                        }
                    }
                }
            }

            return employeModel;
        }



        public async Task<bool> UpdateEmployeeByIdAsync( EmployeModel employee)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateEmployee_New", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", employee.Id);
                    cmd.Parameters.AddWithValue("@CompanyName", employee.CompanyName);
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@Mobile", employee.Mobile);
                    cmd.Parameters.AddWithValue("@Address", employee.Address);
                    cmd.Parameters.AddWithValue("@City", employee.City);
                    cmd.Parameters.AddWithValue("@Pincode", employee.Pincode);

                    await connection.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }




        public async Task<bool> DeactivateEmployeeByIdAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("DeactivateEmployee_New", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
        }













    }
}
