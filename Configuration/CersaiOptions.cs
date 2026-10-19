namespace CersaiCkycApi.Configuration;

public class CersaiOptions
{
    public const string SectionName = "Cersai";

    /// <summary>
    /// Production: https://secureapi.ckycregistry.co.in
    /// Testbed:    https://testbed.secureapi.ckycregistry.co.in
    /// </summary>
    public string BaseUrl { get; set; } = "https://testbed.secureapi.ckycregistry.co.in";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// FI code of the Reporting Entity (reId), max 10 chars.
    /// </summary>
    public string ReId { get; set; } = string.Empty;

    /// <summary>
    /// Path to the client certificate (.pfx / .p12) used for mTLS.
    /// </summary>
    public string ClientCertificatePath { get; set; } = string.Empty;

    /// <summary>
    /// Password for the client certificate (if any).
    /// </summary>
    public string ClientCertificatePassword { get; set; } = string.Empty;

    /// <summary>
    /// Optional path to CERSAI / intermediate CA certificates for server validation.
    /// </summary>
    public string? ServerCaCertificatePath { get; set; }

    /// <summary>
    /// Path or PEM content of the public key used to encrypt request payloads (RSA / JWE).
    /// Actual key material is shared by CERSAI during onboarding.
    /// </summary>
    public string EncryptionPublicKeyPath { get; set; } = string.Empty;

    /// <summary>
    /// Path or PEM content of the private key used to decrypt response payloads.
    /// </summary>
    public string DecryptionPrivateKeyPath { get; set; } = string.Empty;
}
