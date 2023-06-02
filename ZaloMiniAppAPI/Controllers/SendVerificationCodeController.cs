using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendVerificationCodeController : ControllerBase
    {
        private readonly ILogger<SendVerificationCodeController> _logger;

        public SendVerificationCodeController(ILogger<SendVerificationCodeController> logger)
        {
            _logger = logger;
        }

        [HttpPost("send-verification-code")]
        public IActionResult SendVerificationCode([FromBody] ZaloRepons request)
        {
            string phoneNumber = request.PhoneNumber;
            string appId = "2435373010864483579";
            var brandname = Uri.EscapeDataString("Sản Phẩm Thiên Nhiên Trần Mao");
            var redirectUri = Uri.EscapeDataString("http://localhost:2999/callback");
            var zaloAuthorizeUrl = $"https://openapi.zalo.me/v2/oauth/authorize?app_id={appId}&phone={phoneNumber}&brandname={brandname}&redirect_uri={redirectUri}";

            // Trả về một đối tượng RedirectResult để chuyển hướng
            return new RedirectResult(zaloAuthorizeUrl, permanent: false);
        }
    }



}
