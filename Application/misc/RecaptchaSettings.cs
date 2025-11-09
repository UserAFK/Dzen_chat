namespace Application.misc;

public class RecaptchaSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string ExpectedHostname { get; set; } = string.Empty;
}
