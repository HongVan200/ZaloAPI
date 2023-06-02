using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace ZaloMiniAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloMeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ZaloMeController> _logger;

        public ZaloMeController(IHttpClientFactory httpClientFactory, ILogger<ZaloMeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationModel notification)
        {
            try
            {
                // Gọi OpenAPI để gửi thông báo
                var openApiUrl = "https://openapi.mini.zalo.me/notification/template";
                var apiKey = "CwEtA9be0tWrlx8adMW3RpMxlJwLRmngEBAuEJcPvOPbatS0UW";
                var userId = "4595968088514154079";
                var miniAppId = "3642947123294903215";

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
                httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
                httpClient.DefaultRequestHeaders.Add("X-MiniApp-Id", miniAppId);

                // Tạo đối tượng mẫu thông báo
                var template = new
                {
                    template_id = "00126fd75392bacce383",
                    template_data = new
                    {
                        buttonText = "Xem chi tiết đơn hàng",
                        buttonUrl = "https://zalo.me/s/194839900003483517/",
                        title = "Shop Tran Mao - Xác nhận đơn hàng",
                        contentTitle = "Xác nhận đơn hàng",
                        contentDescription = $"Đã nhận được đơn hàng từ {notification.CustomerName}"
                    }
                };

                var response = await httpClient.PostAsJsonAsync(openApiUrl, template);

                if (response.IsSuccessStatusCode)
                {
                    // Xử lý khi gửi thông báo thành công
                    return Ok(new { success = true });
                }
                else
                {
                    // Xử lý khi gửi thông báo không thành công
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send notification: {errorMessage}");
                    return StatusCode(500, new { success = false, error = "Failed to send notification" });
                }
            }
            catch (HttpRequestException ex)
            {
                // Xử lý lỗi khi không thể kết nối đến OpenAPI
                _logger.LogError($"Error connecting to OpenAPI: {ex.Message}");
                return StatusCode(500, new { success = false, error = "Error connecting to OpenAPI" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khác
                _logger.LogError($"Error sending notification: {ex.Message}");
                return StatusCode(500, new { success = false, error = "Error sending notification" });
            }
        }
    }
}
