
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
using ZaloMiniAppAPI;

namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ProductStore _context;
        private readonly UserManager<AccoutDetail> _userManager;
        private readonly IJwtService _jwtService;
        public LoginController(ProductStore context, UserManager<AccoutDetail> userManager, IJwtService jwtService)
        {
            _context = context;
            _userManager = userManager;
            _jwtService = jwtService;
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // Truy vấn cơ sở dữ liệu để tìm tài khoản có số điện thoại đăng nhập tương ứng
            var user = await _context.Set<AccoutDetail>()
                            .SingleOrDefaultAsync(u => u.Sdt == loginDto.Username);


            if (user == null)
            {
                // Số điện thoại không tồn tại trong cơ sở dữ liệu
                return BadRequest("Số điện thoại không tồn tại.");
            }

            // Kiểm tra tính chính xác của mật khẩu
            bool isPasswordCorrect = VerifyPassword(loginDto.Password, user.Password);

            if (!isPasswordCorrect)
            {
                // Mật khẩu không chính xác
                return BadRequest("Mật khẩu không chính xác.");
            }

            var token = _jwtService.GenerateJwtToken(user.CustomerId, user.IsAdminApproved);

            return Ok(new { token });
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                // Mã hóa mật khẩu nhập vào
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedInput = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                // So sánh mật khẩu đã mã hóa với mật khẩu đã lưu trong cơ sở dữ liệu
                return hashedInput == hashedPassword;
            }
        }
    }
}
