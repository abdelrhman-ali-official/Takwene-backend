namespace Shared.SecurityModels;

public class JwtOptions
{
    public string SecretKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public double DurationInDays { get; set; }
}
