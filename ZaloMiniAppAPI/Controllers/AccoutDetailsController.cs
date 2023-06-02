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
    public class AccoutDetailsController : ControllerBase
    {
        private readonly ProductStore _context;

        public AccoutDetailsController(ProductStore context)
        {
            _context = context;
        }

        // GET: api/AccoutDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccoutDetail>>> GetAccoutDetail()
        {
            var nonAdminAccounts = await _context.AccoutDetail
                .Where(a => !a.IsAdminApproved)
                .ToListAsync();



            if (nonAdminAccounts == null)
            {
                return NotFound();
            }

            return nonAdminAccounts;
        }


        // GET: api/AccoutDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccoutDetail>> GetAccoutDetail(int id)
        {
            if (_context.AccoutDetail == null)
            {
                return NotFound();
            }
            var accoutDetail = await _context.AccoutDetail.FindAsync(id);

            if (accoutDetail == null)
            {
                return NotFound();
            }

            return accoutDetail;
        }

        // PUT: api/AccoutDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccoutDetail(int id, AccoutDetail accoutDetail)
        {
            if (id != accoutDetail.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(accoutDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccoutDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AccoutDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AccoutDetail>> PostAcoutDetail(AccoutDetail acoutDetail)
        {
            // Kiểm tra xem tài khoản đã tồn tại trong cơ sở dữ liệu hay chưa
            var existingAccount = await _context.AccoutDetail.FirstOrDefaultAsync(a => a.Sdt == acoutDetail.Sdt);
            if (existingAccount != null)
            {
                return BadRequest("Tài khoản đã tồn tại");
            }

            // Thêm logic xác nhận Admin tại đây
            acoutDetail.RoleId  = "User";

            // Mã hóa mật khẩu
            string hashedPassword = HashPassword(acoutDetail.Password);

            // Cập nhật mật khẩu đã được mã hóa
            acoutDetail.Password = hashedPassword;
            acoutDetail.IsAdminApproved = false;
            // Thêm tài khoản mới vào cơ sở dữ liệu
            _context.AccoutDetail.Add(acoutDetail);
            await _context.SaveChangesAsync();

            // Trả về tài khoản đã tạo thành công
            return CreatedAtAction(nameof(GetAccoutDetail), new { id = acoutDetail.CustomerId }, acoutDetail);
        }

        // DELETE: api/AccoutDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccoutDetail(int id)
        {
            if (_context.AccoutDetail == null)
            {
                return NotFound();
            }
            var accoutDetail = await _context.AccoutDetail.FindAsync(id);
            if (accoutDetail == null)
            {
                return NotFound();
            }

            _context.AccoutDetail.Remove(accoutDetail);
            await _context.SaveChangesAsync();

            return NoContent();
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
