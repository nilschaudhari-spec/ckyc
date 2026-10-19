namespace CersaiCkycApi.Services;

/// <summary>
/// Handles JWE / RSA-256 encryption of request PI data and decryption of response payloads.
/// CERSAI shares the public key (and optionally private key material) during onboarding.
/// Replace the stub implementation with a proper JOSE/JWE library (e.g. jose-jwt, Microsoft.IdentityModel).
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts a plain JSON object into a Base64 JWE string using CERSAI public key.
    /// </summary>
    Task<string> EncryptAsync<T>(T payload, CancellationToken ct = default);

    /// <summary>
    /// Decrypts a Base64 JWE / encryptedData string into the target type.
    /// </summary>
    Task<T?> DecryptAsync<T>(string encryptedData, CancellationToken ct = default);
}
