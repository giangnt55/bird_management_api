namespace AppCore.Extensions;

public static class EnvironmentExtension
{
    public static string GetAppLogFolder() =>
        Environment.GetEnvironmentVariable("LOG_PATH") ?? string.Empty;
    public static string GetAppConnectionString() =>
        Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? string.Empty;

    public static string GetPath() =>
        Environment.GetEnvironmentVariable("DOMAIN_PATH") ?? string.Empty;

    public static string GetJwtIssuer() =>
        Environment.GetEnvironmentVariable("JWT_ISSUER") ?? string.Empty;

    public static string GetJwtAudience() =>
        Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? string.Empty;

    public static string GetJwtAccessTokenSecret() =>
        Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_SECRET") ?? string.Empty;

    public static double GetJwtAccessTokenExpires() =>
        Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRES") ?? "0");

    public static string GetJwtResetTokenSecret() =>
        Environment.GetEnvironmentVariable("JWT_RESET_TOKEN_SECRET") ?? string.Empty;

    public static double GetJwtResetTokenExpires() =>
        Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_RESET_TOKEN_EXPIRES") ?? "0");
}