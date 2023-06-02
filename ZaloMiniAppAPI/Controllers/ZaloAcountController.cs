using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZaloMiniAppAPI;

namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloAcountController : ControllerBase
    {
        private readonly string appId = "YOUR_APP_ID";
        private readonly string appSecret = "YOUR_APP_SECRET";
        private readonly ZaloService _zaloService;

        public ZaloAcountController(ZaloService zaloService)
        {
            _zaloService = zaloService;
        }

        [HttpPost("get-zaloid")]
        public async Task<IActionResult> GetZaloId([FromBody] ZaloRequest request)
        {
            string code = request.Code; // Lấy mã xác thực từ phía client

            // Gửi yêu cầu lấy thông tin người dùng từ Zalo
            var httpClient = new HttpClient();
            var content = new StringContent($"app_id={appId}&app_secret={appSecret}&code={code}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync("https://oauth.zaloapp.com/v3/access_token", content);
            var json = await response.Content.ReadAsStringAsync();

            // Xử lý phản hồi từ Zalo
            dynamic responseObject = JsonConvert.DeserializeObject(json);
            string zaloId = responseObject.data.user_id;

            // Lưu Zalo ID vào cơ sở dữ liệu của ứng dụng
            bool success = _zaloService.LinkAccountWithZalo(zaloId);

            if (success)
            {
                return Ok(zaloId);
            }
            else
            {
                return BadRequest("Failed to link Zalo account");
            }
        }
    }
}
