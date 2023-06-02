using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ProductStore _context;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;
        private readonly IJwtService _jwtService;

        public OrdersController(ProductStore context, IConfiguration configuration, IOptions<JwtSettings> jwtSettings, IJwtService jwtService)
        {
            _context = context;
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;
            _jwtService = jwtService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            // Lấy danh sách đơn hàng theo tài khoản
            int? customerId = GetCustomerIdFromToken();
            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }

            var orders = await _context.Orders.Include(o => o.OrderDetails).Where(o => o.CustomerID == customerId).ToListAsync();

            if (orders == null)
            {
                return NotFound("No orders found.");
            }

            foreach (var order in orders)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = await _context.ProDuctOrders.FindAsync(orderDetail.ProductID);
                    orderDetail.Product = product;
                }
            }

            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            // Lấy đơn hàng theo tài khoản và mã đơn hàng
            int? customerId = GetCustomerIdFromToken();
            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.ID == id && o.CustomerID == customerId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _context.ProDuctOrders.FindAsync(orderDetail.ProductID);
                orderDetail.Product = product;
            }

            return Ok(order);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Orders order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int? customerId = GetCustomerIdFromToken();
            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }

            order.CustomerID = (int)customerId;
            var newOrder = new Orders
            {
                CustomerID = order.CustomerID,
                CustomerName = order.CustomerName,
                PhoneNumber = order.PhoneNumber,
                OrderDate = DateTime.Now,
                Address = order.Address,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDetails = new List<OrderDetail>()
            };

            if (order.OrderDetails != null && order.OrderDetails.Count > 0)
            {
                foreach (var orderProduct in order.OrderDetails)
                {
                    var newOrderProduct = new OrderDetail
                    {
                        OrdersID = newOrder.ID,
                        ProductID = orderProduct.ProductID,
                        Product = orderProduct.Product // Giữ nguyên thông tin sản phẩm từ đơn hàng gửi lên
                    };
                    newOrder.OrderDetails.Add(newOrderProduct);
                }
            }

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Lấy lại đơn hàng với thông tin sản phẩm đầy đủ từ database
            var completeOrder = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.ID == newOrder.ID && o.CustomerID == customerId);

            return CreatedAtAction(nameof(GetOrder), new { id = completeOrder.ID }, completeOrder);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Orders order)
        {
            if (id != order.ID)
            {
                return BadRequest("Invalid order ID.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int? customerId = GetCustomerIdFromToken();
            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }

            var existingOrder = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.ID == id && o.CustomerID == customerId);

            if (existingOrder == null)
            {
                return NotFound("Order not found.");
            }

            existingOrder.CustomerName = order.CustomerName;
            existingOrder.PhoneNumber = order.PhoneNumber;
            existingOrder.Address = order.Address;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.Status = order.Status;

            // Xóa các chi tiết đơn hàng cũ
            _context.OrderDetail.RemoveRange(existingOrder.OrderDetails);

            // Thêm chi tiết đơn hàng mới
            if (order.OrderDetails != null && order.OrderDetails.Count > 0)
            {
                foreach (var orderProduct in order.OrderDetails)
                {
                    var newOrderProduct = new OrderDetail
                    {
                        OrdersID = existingOrder.ID,
                        ProductID = orderProduct.ProductID,
                        Product = orderProduct.Product // Giữ nguyên thông tin sản phẩm từ đơn hàng gửi lên
                    };
                    existingOrder.OrderDetails.Add(newOrderProduct);
                }
            }

            _context.Entry(existingOrder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var orderDetails = await _context.OrderDetail.Where(od => od.OrdersID == id).ToListAsync();
            _context.OrderDetail.RemoveRange(orderDetails);
            await _context.SaveChangesAsync();
            int? customerId = GetCustomerIdFromToken();
            if (customerId == null)
            {
                return BadRequest("Invalid customer ID.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.ID == id && o.CustomerID == customerId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            _context.Orders.Remove(order);
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


    }
}
