using CersaiCkycApi.Models.Create;
using CersaiCkycApi.Models.Download;
using CersaiCkycApi.Models.Search;

namespace CersaiCkycApi.Services;

public interface ICkycClient
{
    // Search
    Task<SearchResponse> SearchIndividualAsync(string searchOption, object plainPayload, string? requestId = null, CancellationToken ct = default);
    Task<SearchResponse> SearchLegalAsync(string searchOption, object plainPayload, string? requestId = null, CancellationToken ct = default);

    // Download
    Task<DownloadInitiateResponse> DownloadInitiateAsync(string ckycType, DownloadInitiatePlain plain, string? requestId = null, CancellationToken ct = default);
    Task<object> DownloadValidateAsync(DownloadValidateRequest request, CancellationToken ct = default);
    Task<OtpValidateResponse> ValidateOtpAsync(string ckycType, string downloadRefId, ValidateOtpPlain otpPayload, string? requestId = null, CancellationToken ct = default);
    Task<object> ResendOtpAsync(string ckycType, string downloadRefId, string? requestId = null, CancellationToken ct = default);
    Task<object> DownloadGetAsync(DownloadGetRequest request, CancellationToken ct = default);

    // Create / Update
    Task<CreateResponse> CreateAsync(string ckycType, object plainCreatePayload, string? requestId = null, CancellationToken ct = default);
    Task<CreateResponse> UpdateAsync(string ckycType, object plainUpdatePayload, string? requestId = null, CancellationToken ct = default);
    Task<object> GetUpdateStatusAsync(string ackNo, CancellationToken ct = default);
    Task<object> FetchUpdatesAsync(string lastUpdatedAfter, int? limit = null, int? page = null, CancellationToken ct = default);
}
