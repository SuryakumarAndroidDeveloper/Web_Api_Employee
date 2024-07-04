using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Models;
using Microsoft.AspNetCore.Mvc;

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
          
            string message = _loginDAL.ExecuteUserAction(model.Email, model.Password);
            if (message == "Login successful")
            {
                string sessionId = _loginDAL.GenerateSessionId();
                return Ok(new { Message = message, SessionId = sessionId });
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
