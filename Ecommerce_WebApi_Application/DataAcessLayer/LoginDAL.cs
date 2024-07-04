using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using MyCaRt.Helper;

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
        public string ExecuteUserAction(string email, string password)
        {
            string message = string.Empty;
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
                        message = "error in login please try later";
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


    }
}
