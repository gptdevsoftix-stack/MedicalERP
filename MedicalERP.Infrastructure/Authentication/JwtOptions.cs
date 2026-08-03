namespace MedicalERP.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "MedicalERP";
    public string Audience { get; set; } = "MedicalERP.Client";
    public string SigningKey { get; set; } = "CHANGE_ME_TO_A_64_CHARACTER_MINIMUM_SECRET_FOR_PRODUCTION";
    public int AccessTokenMinutes { get; set; } = 30;
    public int RefreshTokenDays { get; set; } = 14;
}
