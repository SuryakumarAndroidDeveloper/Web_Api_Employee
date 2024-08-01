using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Ecommerce_WebApi_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly LoginDAL _loginDAL;

        public LoginController(IConfiguration configuration, LoginDAL loginDAL)
        {
            _loginDAL = loginDAL;
        }
        
        [HttpPost]
        [Route("Register")]
        public IActionResult Register(LoginModel model)
        {
            try
            {
                string message = _loginDAL.ExecuteRegisterAction("Register", model.Email, model.Password, model.UserName, model.CPassword);
                if (message == "Registration successful")
                {
                    return Ok(new { Message = message });
                }
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginModel model)
        {
            try
            {         
            string message = _loginDAL.ExecuteUserAction(model.Email, model.Password,out int role, out int uid);
            if (message == "Login successful")
            {
                string sessionId = _loginDAL.GenerateSessionId();
                return Ok(new { Message = message, SessionId = sessionId, Role = role, Userid=uid });
            }
            return Ok(new { Message = message });
        }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("ForgetPassword")]
        public IActionResult ForgetPassword(LoginModel model)
        {
            try
            {
                string message = _loginDAL.ExecuteForgetPasswordAction(model.Email, out string password);
                if (message == "Password Found")
                {
                    return Ok(new { Message = message, Password = password });
                }
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            }

        [HttpPost]
        [Route("SendResetPasswordLink")]
        public async Task<IActionResult> SendResetPasswordLink([FromBody] OtpRequestModel model)
        {
            await  _loginDAL.SaveOtpRequestAsync(model.Email, model.Url);
            return Ok("Reset password link has been saved successfully.");
        }

        [HttpGet("GetOtpRequest")]
        public async Task<IActionResult> GetOtpRequest([FromQuery] string otpUrl)
        {
            var otpRequest = await _loginDAL.GetOtpRequestAsync(otpUrl);
            if (otpRequest == null)
            {
                return NotFound(new { message = "OTP request not found or expired." });
            }
            return Ok(otpRequest);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            try
            {
                string message = _loginDAL.ExecuteResetPasswordAction(model.Email, model.Password, model.CPassword);
                if (message == "Password Updated Successfully")
                {
                    return Ok(new { Message = message });
                }
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
