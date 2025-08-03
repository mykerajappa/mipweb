using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MipWeb.Services;

public class WhatsAppService
{
    private readonly HttpClient _client;
    private readonly string _phoneNumberId = "your_phone_number_id";
    private readonly string _accessToken = "your_access_token";

    public WhatsAppService(HttpClient client)
    {
        _client = client;
    }

    public async Task<bool> SendOtp(string phoneNumber, string otp)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            to = phoneNumber,
            type = "template",
            template = new
            {
                name = "your_template_name",
                language = new { code = "en_US" },
                components = new[]
                {
                    new
                    {
                        type = "body",
                        parameters = new[]
                        {
                            new { type = "text", text = otp }
                        }
                    }
                }
            }
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var request = new HttpRequestMessage(HttpMethod.Post,
            $"https://graph.facebook.com/v19.0/{_phoneNumberId}/messages");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(payload, options), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}
