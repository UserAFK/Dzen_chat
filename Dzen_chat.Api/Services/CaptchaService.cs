namespace Dzen_chat.Api.Services;

public class CaptchaService
{
    private readonly IConfiguration _cfg;
    private readonly HttpClient _http;
    private readonly double minimalScore = 0.5;

    public CaptchaService(IConfiguration cfg, HttpClient http)
    {
        _cfg = cfg;
        _http = http;
    }

    public async Task<bool> VerifyRecaptchaAsync(string token)
    {
        var projectId = _cfg["Recaptcha:ProjectId"];
        var apiKey = _cfg["Recaptcha:ApiKey"];
        var siteKey = _cfg["Recaptcha:SiteKey"];

        var request = new
        {
            @event = new
            {
                token = token,
                siteKey = siteKey
            }
        };

        var response = await _http.PostAsJsonAsync(
            $"https://recaptchaenterprise.googleapis.com/v1/projects/{projectId}/assessments?key={apiKey}",
            request
        );

        var json = await response.Content.ReadFromJsonAsync<RecaptchaEnterpriseResponse>();

        return json?.RiskAnalysis?.Score > minimalScore;
    }

    public class RecaptchaEnterpriseResponse
    {
        public RiskAnalysis RiskAnalysis { get; set; }
        public TokenProperties TokenProperties { get; set; }
    }

    public class RiskAnalysis
    {
        public float Score { get; set; }
        public List<string> Reasons { get; set; }
    }

    public class TokenProperties
    {
        public bool Valid { get; set; }
        public string InvalidReason { get; set; }
        public string Action { get; set; }
    }
}
