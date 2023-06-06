using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.ComponentModel.DataAnnotations;
using ZaloDotNetSDK.entities.shop;
using ZaloMiniAppAPI;
namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class checkAdmin : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public checkAdmin(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpGet]

        // Áp dụng xác thực JWT
        public IActionResult CheckAdmin()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool isAdmin = _jwtService.IsAdmin(token);

            if (isAdmin)
            {
                // User is an admin
                return Ok("admin");
            }
            else
            {
                // User is not an admin
                return Ok("false");
            }
        }
    }
}

