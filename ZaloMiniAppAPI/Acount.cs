using System.ComponentModel.DataAnnotations;

namespace ZaloMiniAppAPI
{
    public class Acount
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

        public string Name   { get; set; }
       public string ZaloId  { get; set; }
    }
}
