using Application.misc;
using Microsoft.Extensions.Options;

namespace Dzen_chat.Api.Services;

public class CaptchaService
{
    private readonly RecaptchaSettings _settings;
    private readonly HttpClient _http;

    public CaptchaService(IOptions<RecaptchaSettings> options, HttpClient http)
    {
        _settings = options.Value;
        _http = http;
    }

    public async Task<bool> VerifyRecaptchaAsync(string token)
    {
        var response = await _http.PostAsync(
            "https://www.google.com/recaptcha/api/siteverify",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "secret", _settings.SecretKey },
                { "response", token }
            }));

        var json = await response.Content.ReadFromJsonAsync<RecaptchaResponse>();
        return json!.Success;
    }

    private record RecaptchaResponse
    {
        public bool Success { get; set; }
        public string Challenge_ts { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
    }
}
