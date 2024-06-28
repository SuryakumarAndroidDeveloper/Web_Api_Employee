namespace Ecommerce_WebApi_Application.Models
{
    public class ProductModel
    {

        public int? Product_Id { get; set; }

        public string? Product_Category { get; set; } 
        public string? Product_Code { get; set; }

        public string? Product_Name { get; set; }

        public decimal? Product_Price { get; set; }

        public string? Product_Description { get; set; }

        public int? Available_Quantity { get; set; }
        public string? FilePath { get; set; }
        public string? ImageName { get; set; }


    }
}
