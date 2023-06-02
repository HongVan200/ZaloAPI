using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ZaloMiniAppAPI
{
    public class ProDuctOrders
    {
       
           
            public int Id { get; set; }
            public string ImagePath { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
      

    }
}
