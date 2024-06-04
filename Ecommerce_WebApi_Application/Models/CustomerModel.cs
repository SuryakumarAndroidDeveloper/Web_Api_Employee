namespace Ecommerce_WebApi_Application.Models
{
    public class CustomerModel
    {
        public int Customer_Id { get; set; }
        public string Customer_FName { get; set; }
        public string Customer_LName { get; set; }
        public string Customer_Gender { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Mobile { get; set; }
        public List<int> SelectedAreas { get; set; }

        public CustomerModel()
        {
            SelectedAreas = new List<int>();
        }

    }
}
