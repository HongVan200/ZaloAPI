using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ZaloMiniAppAPI
{
    public class CartItems
    {
        private decimal price;

        [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //public int CartID { get; set; }
        public int ProductID { get; set; }
        // [MaxLength(3)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //[Range(0,double.MaxValue)]
        public int CategoryID { get; set; }
        // [Range(0, 100)]
        public string ImagePath { get; set; }
        public decimal Price { get => price; set => price = value; }
        public int Quantity { get; set; }
         public int CustomerID { get; set; }
       





    }
}
