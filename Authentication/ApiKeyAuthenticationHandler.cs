using System.Security.Claims;
using System.Text.Encodings.Web;
using CersaiCkycApi.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CersaiCkycApi.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ApiKey";

    private readonly ApiKeyOptions _apiKeyOptions;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<ApiKeyOptions> apiKeyOptions)
        : base(options, logger, encoder)
    {
        _apiKeyOptions = apiKeyOptions.Value;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Allow turning auth off for local debugging
        if (!_apiKeyOptions.Enabled)
        {
            var anonymousIdentity = new ClaimsIdentity(Array.Empty<Claim>(), SchemeName);
            var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(anonymousPrincipal, SchemeName)));
        }

        if (!Request.Headers.TryGetValue(_apiKeyOptions.HeaderName, out var extractedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail($"Missing header: {_apiKeyOptions.HeaderName}"));
        }

        var apiKey = extractedApiKey.ToString();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API key is empty"));
        }

        if (_apiKeyOptions.ValidKeys is null || _apiKeyOptions.ValidKeys.Count == 0)
        {
            Logger.LogError("No ValidKeys configured in ApiKeyAuth section");
            return Task.FromResult(AuthenticateResult.Fail("API key authentication is not configured"));
        }

        if (!_apiKeyOptions.ValidKeys.Contains(apiKey, StringComparer.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "ApiKeyClient"),
            new Claim("api_key", apiKey[..Math.Min(8, apiKey.Length)] + "…") // partial for logging only
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.Headers.WWWAuthenticate = SchemeName;
        Response.ContentType = "application/json";
        return Response.WriteAsync("""{"message":"API key is missing or invalid. Pass a valid key in the X-Api-Key header."}""");
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        Response.ContentType = "application/json";
        return Response.WriteAsync("""{"message":"Forbidden."}""");
    }
}
