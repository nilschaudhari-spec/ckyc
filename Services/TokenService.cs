using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using CersaiCkycApi.Configuration;
using CersaiCkycApi.Models.Common;

namespace CersaiCkycApi.Services;

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CersaiOptions _options;
    private readonly ILogger<TokenService> _logger;

    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public TokenService(
        IHttpClientFactory httpClientFactory,
        IOptions<CersaiOptions> options,
        ILogger<TokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_cachedToken) && DateTimeOffset.UtcNow < _expiresAt.AddSeconds(-30))
        {
            return _cachedToken;
        }

        await _lock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTimeOffset.UtcNow < _expiresAt.AddSeconds(-30))
            {
                return _cachedToken;
            }

            var client = _httpClientFactory.CreateClient("CersaiMtls");
            var url = $"{_options.BaseUrl.TrimEnd('/')}/api/ckyc/v1/generate-token";

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret
            };

            using var content = new FormUrlEncodedContent(form);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            _logger.LogInformation("Requesting OAuth token from CERSAI");

            using var response = await client.PostAsync(url, content, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Token request failed: {Status} {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"CERSAI token request failed: {response.StatusCode}");
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(body)
                ?? throw new InvalidOperationException("Empty token response");

            _cachedToken = tokenResponse.AccessToken;
            var expiresIn = int.TryParse(tokenResponse.ExpiresIn, out var sec) ? sec : 3599;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

            _logger.LogInformation("OAuth token acquired, expires in {Seconds}s", expiresIn);
            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }
}
