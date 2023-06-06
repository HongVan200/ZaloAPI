using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZaloMiniAppAPI;

namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAccount : ControllerBase

    {
        private readonly ProductStore _context;

        public AdminAccount(ProductStore context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccoutDetail>>> GetAdminAccount()
        {
            var adminAccounts = await _context.AccoutDetail
                .Where(a => a.IsAdminApproved)
                .ToListAsync();

            if (adminAccounts == null)
            {
                return NotFound();
            }

            return adminAccounts;
        }
        [HttpPost]
        public async Task<ActionResult<AccoutDetail>> PostAdminAccount(AccoutDetail acoutDetail)
        {


            // Kiểm tra xem tài khoản đã tồn tại trong cơ sở dữ liệu hay chưa
            var existingAccount = await _context.AccoutDetail.FirstOrDefaultAsync(a => a.UserID == acoutDetail.UserID);
            if (existingAccount != null)
            {
                return BadRequest("Tài khoản đã tồn tại");
            }
            
                // Thêm logic xác nhận Admin tại đây
                acoutDetail.IsAdminApproved = true;
            
          
            _context.AccoutDetail.Add(acoutDetail);
            await _context.SaveChangesAsync();

            // Trả về tài khoản đã tạo thành công
            return CreatedAtAction(nameof(GetAdminAccount), new { id = acoutDetail.CustomerId }, acoutDetail);


        }
        private bool AccoutDetailExists(int id)
        {
            return (_context.AccoutDetail?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

    }
}
