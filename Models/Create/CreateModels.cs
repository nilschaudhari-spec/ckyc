using System.Text.Json.Serialization;

namespace CersaiCkycApi.Models.Create;

/// <summary>
/// Outer envelope for Create API.
/// The rich KYC payload is serialized, encrypted, and placed in CkycInq.Data / EncryptedData.
/// Full field list is very large; this model covers the envelope + key top-level structures.
/// Expand personalDetails / entityDetails as required for your use-cases.
/// </summary>
public class CreateRequestEnvelope
{
    [JsonPropertyName("reId")]
    public string ReId { get; set; } = string.Empty;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("ckycInq")]
    public CreateInquiry CkycInq { get; set; } = new();
}

public class CreateInquiry
{
    [JsonPropertyName("ckycType")]
    public string CkycType { get; set; } = string.Empty;

    /// <summary>
    /// Encrypted payload (Base64 JWE). Contains the full Individual or Legal create body.
    /// </summary>
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}

/// <summary>
/// Plain Individual create body (to be encrypted before sending).
/// Only the most important sections are modelled; add remaining fields from the PDF as needed.
/// </summary>
public class IndividualCreatePayload
{
    [JsonPropertyName("searchKey")]
    public string SearchKey { get; set; } = string.Empty;

    [JsonPropertyName("accountType")]
    public string AccountType { get; set; } = "NORMAL"; // NORMAL | MINOR | SMALL | AADHAR_EKYC

    [JsonPropertyName("personalDetails")]
    public PersonalDetails? PersonalDetails { get; set; }

    // Additional sections (photo, identityProofs, addressDetails, contactDetails, etc.)
    // should be added following the Create API section of the guide.
}

public class PersonalDetails
{
    [JsonPropertyName("name")]
    public NameDetail? Name { get; set; }

    [JsonPropertyName("familyDetails")]
    public FamilyDetails? FamilyDetails { get; set; }

    [JsonPropertyName("dob")]
    public string? Dob { get; set; }

    [JsonPropertyName("isMinor")]
    public bool IsMinor { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("panDetails")]
    public PanDetails? PanDetails { get; set; }
}

public class NameDetail
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
}

public class FamilyDetails
{
    [JsonPropertyName("father")]
    public NameDetail? Father { get; set; }

    [JsonPropertyName("mother")]
    public NameDetail? Mother { get; set; }

    [JsonPropertyName("spouse")]
    public NameDetail? Spouse { get; set; }

    [JsonPropertyName("maidenName")]
    public NameDetail? MaidenName { get; set; }
}

public class PanDetails
{
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("form97Submitted")]
    public bool Form97Submitted { get; set; }
}

/// <summary>
/// Minimal Legal create payload placeholder.
/// Full structure is defined in the Create API section for LEGAL entities.
/// </summary>
public class LegalCreatePayload
{
    [JsonPropertyName("searchKey")]
    public string SearchKey { get; set; } = string.Empty;

    [JsonPropertyName("entityName")]
    public string? EntityName { get; set; }

    [JsonPropertyName("constitution")]
    public string? Constitution { get; set; }

    [JsonPropertyName("doi")]
    public string? Doi { get; set; }
}

public class CreateResponse
{
    [JsonPropertyName("reId")]
    public string? ReId { get; set; }

    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }

    [JsonPropertyName("ackNo")]
    public string? AckNo { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
