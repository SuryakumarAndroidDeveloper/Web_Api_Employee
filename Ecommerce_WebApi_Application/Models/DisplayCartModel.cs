using System.ComponentModel.DataAnnotations;

namespace Ecommerce_WebApi_Application.Models
{
    public class DisplayCartModel
    {

       
        public int? CartItem_Id { get; set; }

   
        public int? Product_Id { get; set; }


        public string? Product_Name { get; set; }

        public int? Quantity { get; set; }


        public int? Product_Price { get; set; }




    }
}
