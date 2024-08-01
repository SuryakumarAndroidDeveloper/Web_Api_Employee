namespace Ecommerce_WebApi_Application.Models
{
    public class CustomerInterestedCategory
    {
        public int Customer_Id { get; set; }
        public string Customer_FName { get; set; }
        public string Customer_LName { get; set; }
        public string Customer_Gender { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Mobile { get; set; }

        public string Customer_InterestedCategory { get; set; }

        public string? FilePath { get; set; }
        public string? ImageName { get; set; }

    }
}
