using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Yêu cầu xác thực người dùng
    public class CartController : ControllerBase
    {
        private readonly ProductStore _context;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;
        private readonly IJwtService _jwtService;

        public CartController(ProductStore context, IConfiguration configuration, IOptions<JwtSettings> jwtSettings, IJwtService jwtService)
        {
            _context = context;
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;
            _jwtService = jwtService;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItems>>> GetCartItems()
        {
            var customerId = GetCustomerIdFromToken();

            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }

            var cartItems = await _context.CartItems.Where(c => c.CustomerID == customerId).ToListAsync();
            return Ok(cartItems);
        }

        // GET: api/Cart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItems>> GetCartItems(int id)
        {
            var customerId = GetCustomerIdFromToken();

            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }

            var cartItems = await _context.CartItems.FirstOrDefaultAsync(c => c.CustomerID == customerId && c.CustomerID == id);

            if (cartItems == null)
            {
                return NotFound();
            }

            return Ok(cartItems);
        }

        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItems(int id, CartItems cartItems)
        {
            if (cartItems.CustomerID != id)
            {
                return BadRequest("Invalid order ID.");
            }

            _context.Entry(cartItems).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemsExists(id))
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

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<CartItems>> PostCartItems(CartItems cartItems)
        {
            var customerId = GetCustomerIdFromToken();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }

            cartItems.CustomerID = customerId.Value;

            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.CustomerID == customerId && c.ProductID == cartItems.ProductID);
            if (existingCartItem != null)
            {
                // Sản phẩm đã tồn tại trong giỏ hàng, tăng số lượng sản phẩm
                existingCartItem.Quantity += cartItems.Quantity;
                _context.CartItems.Update(existingCartItem);
            }
            else
            {
                // Sản phẩm chưa tồn tại trong giỏ hàng, thêm mới
                _context.CartItems.Add(cartItems);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCartItems", new { id = cartItems.CustomerID }, cartItems);
        }

        // DELETE: api/Cart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItems(int id)
        {
            var customerId = GetCustomerIdFromToken();

            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }

            var cartItems = await _context.CartItems.FirstOrDefaultAsync(c => c.CustomerID == customerId && c.ID == id);

            if (cartItems == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private int? GetCustomerIdFromToken()
        {
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                if (_jwtService.ValidateJwtToken(token))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(token);
                    var customerIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;

                    if (int.TryParse(customerIdClaim, out int customerId))
                    {
                        return customerId;
                    }
                }
            }

            return null;
        }

        private bool CartItemsExists(int id)
        {
            return _context.CartItems.Any(e => e.CustomerID == id);
        }
    }
}
