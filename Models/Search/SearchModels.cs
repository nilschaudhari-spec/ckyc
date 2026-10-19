using System.Text.Json.Serialization;

namespace CersaiCkycApi.Models.Search;

// ---------- Individual search PI (goes inside encryptedData) ----------

public class IndividualOvdSearchPayload
{
    [JsonPropertyName("ovdDetails")]
    public List<OvdDetail> OvdDetails { get; set; } = new();
}

public class OvdDetail
{
    [JsonPropertyName("ovdType")]
    public string OvdType { get; set; } = string.Empty;

    [JsonPropertyName("ovdNo")]
    public string OvdNo { get; set; } = string.Empty;

    /// <summary>Required when ovdType = E (Aadhaar)</summary>
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("dob")]
    public string? Dob { get; set; }   // DD-MM-YYYY

    [JsonPropertyName("gender")]
    public string? Gender { get; set; } // M | F | T
}

public class IndividualCkycSearchPayload
{
    [JsonPropertyName("ckycNo")]
    public string CkycNo { get; set; } = string.Empty; // 14 digits
}

public class IndividualDemographicSearchPayload
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("dob")]
    public string Dob { get; set; } = string.Empty;

    [JsonPropertyName("relation")]
    public string Relation { get; set; } = string.Empty; // Father | Mother | Spouse

    [JsonPropertyName("relativeFirstName")]
    public string RelativeFirstName { get; set; } = string.Empty;

    [JsonPropertyName("relativeMiddleName")]
    public string? RelativeMiddleName { get; set; }

    [JsonPropertyName("relativeLastName")]
    public string? RelativeLastName { get; set; }
}

public class IndividualPhotoSearchPayload
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("b64Photo")]
    public string B64Photo { get; set; } = string.Empty;
}

public class IndividualMobileSearchPayload
{
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = string.Empty; // 10 digits, not starting with 0
}

public class IndividualVcSearchPayload
{
    [JsonPropertyName("ckycAlias")]
    public string CkycAlias { get; set; } = string.Empty;
}

// ---------- Legal search PI ----------

public class LegalOvdSearchPayload
{
    [JsonPropertyName("documentIds")]
    public List<LegalDocumentId> DocumentIds { get; set; } = new();
}

public class LegalDocumentId
{
    [JsonPropertyName("id_type")]
    public string IdType { get; set; } = string.Empty; // C, 01, 02, 04

    [JsonPropertyName("id_number")]
    public string IdNumber { get; set; } = string.Empty;
}

public class LegalDoiSearchPayload
{
    [JsonPropertyName("constitution")]
    public string Constitution { get; set; } = string.Empty;

    [JsonPropertyName("entityName")]
    public string EntityName { get; set; } = string.Empty;

    [JsonPropertyName("doi")]
    public string Doi { get; set; } = string.Empty; // DD-MM-YYYY
}

public class LegalMobileSearchPayload
{
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = string.Empty;
}

// ---------- Search response (decrypted) ----------

public class SearchResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public SearchData? Data { get; set; }
}

public class SearchData
{
    [JsonPropertyName("searchKey")]
    public string? SearchKey { get; set; }

    [JsonPropertyName("results")]
    public List<SearchResultItem>? Results { get; set; }
}

public class SearchResultItem
{
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; } // Legal

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("photo")]
    public string? Photo { get; set; }

    [JsonPropertyName("photoExt")]
    public string? PhotoExt { get; set; }

    [JsonPropertyName("docSubmitted")]
    public List<DocSubmitted>? DocSubmitted { get; set; }

    [JsonPropertyName("lastUpdateDate")]
    public string? LastUpdateDate { get; set; }

    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("ckycRefNo")]
    public string? CkycRefNo { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    [JsonPropertyName("matchedOvdType")]
    public string? MatchedOvdType { get; set; }

    [JsonPropertyName("matchedOvdNumber")]
    public string? MatchedOvdNumber { get; set; }

    [JsonPropertyName("cin")]
    public string? Cin { get; set; }

    [JsonPropertyName("dateOfRegistration")]
    public string? DateOfRegistration { get; set; }

    [JsonPropertyName("deactivationReason")]
    public string? DeactivationReason { get; set; }
}

public class DocSubmitted
{
    [JsonPropertyName("docName")]
    public string? DocName { get; set; }

    [JsonPropertyName("docType")]
    public string? DocType { get; set; }
}
