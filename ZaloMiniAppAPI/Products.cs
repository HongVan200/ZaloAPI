using System.ComponentModel.DataAnnotations;



namespace ZaloMiniAppAPI
{
    public class Products
    {

        [Key]
        public int ProductID { get; set; }
        // [MaxLength(3)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //[Range(0,double.MaxValue)]
        public int CategoryID { get; set; }
       // [Range(0, 100)]
        public string ImagePath { get ; set; }
        private decimal _price;
        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
           
        }

        public string FormattedPrice
        {
            get { return _price.ToString("N0"); }
        }
        

    }

    internal class MaxLenghtAttribute : Attribute
    {
    }
}
