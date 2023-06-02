using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZaloMiniAppAPI;
using Microsoft.AspNetCore.Cors;

namespace ZaloMiniAppAPI.Controllers
{
    [EnableCors("DefaultPolicy")]

    [Route("api/[controller]")]
    [ApiController]
    public class ProductListsController : ControllerBase
    {
        private readonly ProductStore _context;

        public ProductListsController(ProductStore context)
        {
            _context = context;
        }

        // GET: api/ProductLists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProductList() 
        {
            if (_context.Products == null)
            {
                return NotFound();
            }

            var products = await _context.Products.ToListAsync();

            // Thêm tiêu đề CORS vào phản hồi
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:2999");

            return products;
        }

        // GET: api/ProductLists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProductList(int id)
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            var productList = await _context.Products!.FindAsync(id);

            if (productList == null)
            {
                return NotFound();
            }

            return productList;
        }

        // PUT: api/ProductLists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductList(int id, Products productList)
        {
            if (id != productList.ProductID)
            {
                return BadRequest();
            }

            _context.Entry(productList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductListExists(id))
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

        // POST: api/ProductLists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Products>> PostProductList(Products productList)
        {
          if (_context.Products == null)
          {
              return Problem("Entity set 'ProductStore.ProductList'  is null.");
          }
            _context.Products!.Add(productList);
            await _context.SaveChangesAsync();
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:2999");
            return CreatedAtAction("GetProductList", new { id = productList.ProductID }, productList);
           
        }

        // DELETE: api/ProductLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductList(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var productList = await _context.Products.FindAsync(id);
            if (productList == null)
            {
                return NotFound();
            }

            _context.Products!.Remove(productList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductListExists(int id)
        {
            return (_context.Products?.Any(e => e.ProductID == id)).GetValueOrDefault();
        }
    }
}
