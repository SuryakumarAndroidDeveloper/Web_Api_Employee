using System.ComponentModel.DataAnnotations;

namespace Ecommerce_WebApi_Application.Models
{
    public class ResetPasswordModel
    {
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? CPassword { get; set; }
    }
}
