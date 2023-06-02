using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZaloMiniAppAPI
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        [ForeignKey("Order")]
        public int OrdersID { get; set; }
        //public Orders Order { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public ProDuctOrders? Product { get; set; }

       
    }
}
