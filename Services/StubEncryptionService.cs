using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using CersaiCkycApi.Configuration;

namespace CersaiCkycApi.Services;

/// <summary>
/// DEVELOPMENT STUB ONLY.
/// In production replace with real RSA-OAEP / JWE encryption using the keys
/// provided by CERSAI. Never send real PI data with this stub.
/// </summary>
public class StubEncryptionService : IEncryptionService
{
    private readonly ILogger<StubEncryptionService> _logger;

    public StubEncryptionService(IOptions<CersaiOptions> options, ILogger<StubEncryptionService> logger)
    {
        _logger = logger;
        // In real impl: load public/private keys from options.EncryptionPublicKeyPath etc.
    }

    public Task<string> EncryptAsync<T>(T payload, CancellationToken ct = default)
    {
        // WARNING: This is NOT real encryption – it only Base64-encodes the JSON
        // so the rest of the pipeline can be tested. Replace before any production use.
        var json = JsonSerializer.Serialize(payload);
        var bytes = Encoding.UTF8.GetBytes(json);
        var encoded = Convert.ToBase64String(bytes);
        _logger.LogWarning("StubEncryptionService used – payload is only Base64 encoded, not encrypted with RSA/JWE.");
        return Task.FromResult(encoded);
    }

    public Task<T?> DecryptAsync<T>(string encryptedData, CancellationToken ct = default)
    {
        try
        {
            var bytes = Convert.FromBase64String(encryptedData);
            var json = Encoding.UTF8.GetString(bytes);
            var result = JsonSerializer.Deserialize<T>(json);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt payload with stub service");
            return Task.FromResult<T?>(default);
        }
    }
}
