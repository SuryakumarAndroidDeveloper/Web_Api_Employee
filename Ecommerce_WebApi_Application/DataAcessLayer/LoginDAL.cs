using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using MyCaRt.Helper;
using Ecommerce_WebApi_Application.Models;

namespace Ecommerce_WebApi_Application.DataAcessLayer
{
    public class LoginDAL
    {
        private readonly IConfiguration _configuration;

        public LoginDAL(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Register the user dal
        public string ExecuteRegisterAction(string action, string email, string password, string userName = null, string cpassword=null)
        {
            string message = string.Empty;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("UserLoginRegister", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                   
                    if (!string.IsNullOrEmpty(userName))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);
                    }
                    if (!string.IsNullOrEmpty(cpassword))
                    {
                        cmd.Parameters.AddWithValue("@CPassword", cpassword);
                    }

                    con.Open();
                    message = cmd.ExecuteScalar().ToString();
                    con.Close();
                }
            }
            return message;
        }
        //login authentication
        public string ExecuteUserAction(string email, string password, out int role, out int uid)
        {
            string message = string.Empty;
            role = -1; 
            uid = -1;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("UserLogin", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string storedPassword = reader["Password"].ToString();
                        role = (int)reader["Role"];
                        uid = Convert.ToInt32(reader["Customer_Id"]);
                        if (EncryptionHelper.Compare(password, storedPassword))
                        {
                            message = "Login successful";
                        }
                        else
                        {
                            message = "Invalid email or password";
                        }
                    }
                    else
                    {
                        message = "Account not Exists Please signup to login!";
                    }
                    con.Close();
                }
            }
            return message;
        }
        public string GenerateSessionId()
        {
            using (var cryptoProvider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[16];
                cryptoProvider.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }


        //forget password action

        public string ExecuteForgetPasswordAction(string email, out string password)
        {
            string message = string.Empty;
            password = null;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("ForgetPassword", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        password = reader["Password"].ToString();
                         password = EncryptionHelper.DecryptString(password);

                        message = "Password Found";
                    }
                    else
                    {
                        message = "Email Not Found. Please check your email address.";
                    }
                    con.Close();
                }
            }
            return message;
        }


        //reset password

        public string ExecuteResetPasswordAction(string email, string password, string cPassword)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("ResetPassword", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@CPassword", cPassword);

                    SqlParameter returnValue = new SqlParameter();
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        int result = (int)returnValue.Value;

                        if (result == 0)
                        {
                            return "Password Updated Successfully";
                        }
                        else
                        {
                            return "Email not found";
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"Error: {ex.Message}";
                    }
                }
            }
        }

        //save the url to database
        public async Task SaveOtpRequestAsync(string email, string otpUrl)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
               // string query = "INSERT INTO OtpRequests (Email, OtpUrl, CreatedAt) VALUES (@Email, @OtpUrl, @CreatedAt)";
                using (SqlCommand command = new SqlCommand("SaveResetUrl", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@OtpUrl", otpUrl);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        //get the email and url based on the browser url
        public async Task<OtpRequestModel> GetOtpRequestAsync(string otpUrl)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
               // string query = "SELECT Email, OtpUrl FROM OtpRequests WHERE OtpUrl = @OtpUrl";
                using (SqlCommand command = new SqlCommand("RetriveResetUrl", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OtpUrl", otpUrl);
                    connection.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            return new OtpRequestModel
                            {
                                Email = reader["Email"].ToString(),
                                Url = reader["OtpUrl"].ToString(),
                                CreatedAt = (DateTime)reader["CreatedAt"]
                            };
                        }
                    }
                }
            }
            return null;
        }


        public async Task CleanupExpiredOtpsAsync()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                //string query = "DELETE FROM OtpRequests WHERE CreatedAt < @ExpirationTime";
                using (SqlCommand command = new SqlCommand("CleanSavedResetUrl", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExpirationTime", DateTime.UtcNow.AddMinutes(-5));
                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
            }


        }
}
