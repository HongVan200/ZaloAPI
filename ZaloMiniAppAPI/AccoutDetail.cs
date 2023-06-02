using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ZaloMiniAppAPI
{
    public class AccoutDetail:IdentityUser<string>
    {
        [Key]
        public int CustomerId { get; set; }

        public string? Sdt { get; set; }
        public string? Password { get; set; }
        public bool IsAdminApproved { get; set; }
        public string? RoleId { get; set; }
    }
}
