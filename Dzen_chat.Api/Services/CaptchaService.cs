namespace Dzen_chat.Api.Services;

internal static class CaptchaService
{
    internal static async Task<bool> VerifyRecaptchaAsync(string token)
    {
        var secretKey = "6LfeggQsAAAAANszxO-prC6lvRPIIC_4bR_nITPq";
        var hostname = "localhost";
        using var http = new HttpClient();
        var response = await http.PostAsync(
            "https://www.google.com/recaptcha/api/siteverify",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", token }
            }));

        var json = await response.Content.ReadFromJsonAsync<RecaptchaResponse>();
        return json.Success && json.Hostname == hostname;
    }

    private record RecaptchaResponse
    {
        public bool Success { get; set; }
        public string Challenge_ts { get; set; }
        public string Hostname { get; set; }
    }
}
