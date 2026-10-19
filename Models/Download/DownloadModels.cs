using System.Text.Json.Serialization;

namespace CersaiCkycApi.Models.Download;

// ---------- Initiate ----------

public class DownloadInitiatePlain
{
    [JsonPropertyName("ckycNo")]
    public string? CkycNo { get; set; }

    [JsonPropertyName("ckycRefNo")]
    public string? CkycRefNo { get; set; }

    [JsonPropertyName("ckycAlias")]
    public string? CkycAlias { get; set; }
}

public class DownloadInitiateResponse
{
    [JsonPropertyName("reId")]
    public string? ReId { get; set; }

    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public DownloadInitiateData? Data { get; set; }
}

public class DownloadInitiateData
{
    [JsonPropertyName("ckycRefNo")]
    public string? CkycRefNo { get; set; }

    [JsonPropertyName("hasMobileNumber")]
    public bool HasMobileNumber { get; set; }

    [JsonPropertyName("hasEmailAddress")]
    public bool HasEmailAddress { get; set; }

    [JsonPropertyName("consentCheck")]
    public bool ConsentCheck { get; set; }

    [JsonPropertyName("status")]
    public bool Status { get; set; }

    [JsonPropertyName("downloadRefId")]
    public string? DownloadRefId { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

// ---------- Validate (consent) ----------

public class DownloadValidateRequest
{
    [JsonPropertyName("reId")]
    public string ReId { get; set; } = string.Empty;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("downloadRefId")]
    public string DownloadRefId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("ckycInq")]
    public DownloadValidateInquiry CkycInq { get; set; } = new();
}

public class DownloadValidateInquiry
{
    [JsonPropertyName("ckycType")]
    public string CkycType { get; set; } = string.Empty;

    [JsonPropertyName("consentType")]
    public string ConsentType { get; set; } = string.Empty;

    [JsonPropertyName("encryptedData")]
    public string? EncryptedData { get; set; }
}

public class PhysicalAuthFactorsPayload
{
    [JsonPropertyName("authFactors")]
    public List<AuthFactor> AuthFactors { get; set; } = new();
}

public class AuthFactor
{
    [JsonPropertyName("authFactor")]
    public string AuthFactorType { get; set; } = string.Empty; // DOB | RELATION | BIRTHYEARPINCODE

    [JsonPropertyName("authValue")]
    public string? AuthValue { get; set; }

    [JsonPropertyName("authFactorValueObj")]
    public AuthFactorValueObj? AuthFactorValueObj { get; set; }
}

public class AuthFactorValueObj
{
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("relationName")]
    public string? RelationName { get; set; }

    [JsonPropertyName("year")]
    public string? Year { get; set; }

    [JsonPropertyName("pincode")]
    public string? Pincode { get; set; }
}

// ---------- OTP ----------

public class ValidateOtpPlain
{
    [JsonPropertyName("otp")]
    public string Otp { get; set; } = string.Empty;

    [JsonPropertyName("otpIdentifier")]
    public string OtpIdentifier { get; set; } = string.Empty;
}

public class OtpValidateResponse
{
    [JsonPropertyName("reId")]
    public string? ReId { get; set; }

    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }

    [JsonPropertyName("downloadRefId")]
    public string? DownloadRefId { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("status")]
    public bool Status { get; set; }
}

// ---------- Final GET ----------

public class DownloadGetRequest
{
    [JsonPropertyName("reId")]
    public string ReId { get; set; } = string.Empty;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("downloadRefId")]
    public string DownloadRefId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("ckycInq")]
    public DownloadGetInquiry CkycInq { get; set; } = new();
}

public class DownloadGetInquiry
{
    [JsonPropertyName("ckycType")]
    public string CkycType { get; set; } = string.Empty;
}
