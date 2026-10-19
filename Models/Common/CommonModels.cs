using System.Text.Json.Serialization;

namespace CersaiCkycApi.Models.Common;

/// <summary>
/// Common outer envelope used by most CERSAI APIs.
/// Sensitive PI data goes inside EncryptedData (JWE / RSA-256).
/// </summary>
public class CkycRequestEnvelope
{
    [JsonPropertyName("reId")]
    public string ReId { get; set; } = string.Empty;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("ckycInq")]
    public CkycInquiry? CkycInq { get; set; }
}

public class CkycInquiry
{
    [JsonPropertyName("ckycType")]
    public string CkycType { get; set; } = string.Empty;

    [JsonPropertyName("searchOption")]
    public string? SearchOption { get; set; }

    [JsonPropertyName("consentType")]
    public string? ConsentType { get; set; }

    [JsonPropertyName("encryptedData")]
    public string? EncryptedData { get; set; }
}

public class TokenResponse
{
    [JsonPropertyName("issuedAt")]
    public string? IssuedAt { get; set; }

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expiresIn")]
    public string? ExpiresIn { get; set; }
}

public class ApiErrorResponse
{
    [JsonPropertyName("success")]
    public bool? Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }
}
