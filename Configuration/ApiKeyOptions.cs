namespace CersaiCkycApi.Configuration;

public class ApiKeyOptions
{
    public const string SectionName = "ApiKeyAuth";

    /// <summary>
    /// Header name that carries the API key (default: X-Api-Key).
    /// </summary>
    public string HeaderName { get; set; } = "X-Api-Key";

    /// <summary>
    /// Valid API keys. Prefer storing these in secrets / environment variables in production.
    /// </summary>
    public List<string> ValidKeys { get; set; } = new();

    /// <summary>
    /// When false, authentication is skipped (useful for local debugging only).
    /// </summary>
    public bool Enabled { get; set; } = true;
}
