using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ZaloMiniAppAPI
{
    public class AccoutDetail
    {
        [Key]
        public int CustomerId { get; set; }
        public long UserID { get; set; }
        public string? Username { get; set; }
        public bool IsAdminApproved { get; set; }
        public string? RoleId { get; set; }
    }
}
