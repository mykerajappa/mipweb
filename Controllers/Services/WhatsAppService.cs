using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace MipWeb.Services
{
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly string _phoneNumberId;
        private readonly string _apiVersion;

        public WhatsAppService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _accessToken = configuration["WhatsApp:AccessToken"];
            _phoneNumberId = configuration["WhatsApp:PhoneNumberId"];
            _apiVersion = configuration["WhatsApp:ApiVersion"];
        }

        public async Task<bool> SendOtp(string phoneNumber, string otpCode)
        {
            var url = $"https://graph.facebook.com/{_apiVersion}/{_phoneNumberId}/messages";

            var payload = new
            {
                messaging_product = "whatsapp",
                to = phoneNumber,
                type = "template",
                template = new
                {
                    name = "otp_template", // use your approved template name
                    language = new { code = "en_US" },
                    components = new[]
                    {
                        new
                        {
                            type = "body",
                            parameters = new[]
                            {
                                new { type = "text", text = otpCode }
                            }
                        }
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
