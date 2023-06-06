
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZaloDotNetSDK.entities.shop;
using ZaloMiniAppAPI;

namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ProductStore _context;
        private readonly IJwtService _jwtService;
        public LoginController(ProductStore context, IJwtService jwtService)
        {
            _context = context;
           
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccoutDetail>>> GetAllAccounts()
        {
            var accounts = await _context.AccoutDetail.ToListAsync();
            return Ok(accounts);
        }
        [HttpPost]
        public async Task<IActionResult> Login(AccoutDetail accoutDetail)
        {
            var existingUser = await _context.AccoutDetail.FirstOrDefaultAsync(u => u.UserID == accoutDetail.UserID);

            if (existingUser == null)
            {
                // Thêm tài khoản mới nếu UserID chưa tồn tại trong cơ sở dữ liệu
                var user = new AccoutDetail
                {
                    UserID = accoutDetail.UserID,
                    Username = accoutDetail.Username,
                    IsAdminApproved =false,
                    RoleId = "User"
                };

                _context.AccoutDetail.Add(user);
                await _context.SaveChangesAsync();

                // Tạo mã token với CustomerId vừa tạo
                var newToken = _jwtService.GenerateJwtToken(user.CustomerId, user.IsAdminApproved);
                return Ok(new { token = newToken });
            }
            else
            {
                // Đăng nhập và trả về mã token nếu UserID đã tồn tại trong cơ sở dữ liệu
                var token = _jwtService.GenerateJwtToken(existingUser.CustomerId, existingUser.IsAdminApproved);
                return Ok(new { token });
            }
        }

    }
}
