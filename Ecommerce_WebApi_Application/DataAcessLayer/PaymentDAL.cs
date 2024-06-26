using Ecommerce_WebApi_Application.Models;
using System.Data.SqlClient;
using System.Data;

namespace Ecommerce_WebApi_Application.DataAcessLayer
{
    public class PaymentDAL
    {

        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public PaymentDAL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int?> StorePaymentData(PaymentModel paymentModel)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("StorePaymentData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        //command.Parameters.AddWithValue("@CustomerId", paymentModel.CustomerId);
                        command.Parameters.AddWithValue("@FullName", paymentModel.FullName);
                        command.Parameters.AddWithValue("@Email", paymentModel.Email);
                        command.Parameters.AddWithValue("@Address", paymentModel.Address);
                        command.Parameters.AddWithValue("@City", paymentModel.City);
                        command.Parameters.AddWithValue("@State", paymentModel.State);
                        command.Parameters.AddWithValue("@ZipCode", paymentModel.ZipCode);
                        command.Parameters.AddWithValue("@CardName", paymentModel.CardName);
                        command.Parameters.AddWithValue("@CardNumber", paymentModel.CardNumber);
                        command.Parameters.AddWithValue("@ExpMonth", paymentModel.ExpMonth);
                        command.Parameters.AddWithValue("@ExpYear", paymentModel.ExpYear);
                        command.Parameters.AddWithValue("@CVV", paymentModel.CVV);

                        // Add an output parameter to capture the paymentId
                        SqlParameter outputIdParam = new SqlParameter("@PaymentId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputIdParam);
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        //var result = await command.ExecuteNonQueryAsync();
                        //return result > 0;
                        //return true;
                        // Retrieve the paymentId
                        int paymentId = (int)outputIdParam.Value;
                        return paymentId;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("Error storing payment data: " + ex.Message);
                // return false;
                return null;
            }
        }
    }
}

