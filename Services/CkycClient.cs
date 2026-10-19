using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using CersaiCkycApi.Configuration;
using CersaiCkycApi.Models.Common;
using CersaiCkycApi.Models.Create;
using CersaiCkycApi.Models.Download;
using CersaiCkycApi.Models.Search;

namespace CersaiCkycApi.Services;

public class CkycClient : ICkycClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenService _tokenService;
    private readonly IEncryptionService _encryption;
    private readonly CersaiOptions _options;
    private readonly ILogger<CkycClient> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public CkycClient(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        IEncryptionService encryption,
        IOptions<CersaiOptions> options,
        ILogger<CkycClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
        _encryption = encryption;
        _options = options.Value;
        _logger = logger;
    }

    // ----------------- Search -----------------

    public Task<SearchResponse> SearchIndividualAsync(string searchOption, object plainPayload, string? requestId = null, CancellationToken ct = default)
        => SearchAsync(CkycType.Individual, searchOption, plainPayload, requestId, ct);

    public Task<SearchResponse> SearchLegalAsync(string searchOption, object plainPayload, string? requestId = null, CancellationToken ct = default)
        => SearchAsync(CkycType.Legal, searchOption, plainPayload, requestId, ct);

    private async Task<SearchResponse> SearchAsync(string ckycType, string searchOption, object plainPayload, string? requestId, CancellationToken ct)
    {
        var encrypted = await _encryption.EncryptAsync(plainPayload, ct);

        var envelope = new CkycRequestEnvelope
        {
            ReId = _options.ReId,
            RequestId = requestId ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            CkycInq = new CkycInquiry
            {
                CkycType = ckycType,
                SearchOption = searchOption,
                EncryptedData = encrypted
            }
        };

        var responseJson = await PostAsync("/api/ckyc/v1/search", envelope, ct);
        return JsonSerializer.Deserialize<SearchResponse>(responseJson, JsonOpts)
               ?? new SearchResponse { Success = false, Message = "Empty response" };
    }

    // ----------------- Download -----------------

    public async Task<DownloadInitiateResponse> DownloadInitiateAsync(string ckycType, DownloadInitiatePlain plain, string? requestId = null, CancellationToken ct = default)
    {
        var encrypted = await _encryption.EncryptAsync(plain, ct);

        var envelope = new
        {
            reId = _options.ReId,
            requestId = requestId ?? Guid.NewGuid().ToString(),
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            ckycInq = new
            {
                ckycType,
                encryptedData = encrypted
            }
        };

        var responseJson = await PostAsync("/api/ckyc/v1/download/initiate", envelope, ct);
        return JsonSerializer.Deserialize<DownloadInitiateResponse>(responseJson, JsonOpts)
               ?? new DownloadInitiateResponse();
    }

    public async Task<object> DownloadValidateAsync(DownloadValidateRequest request, CancellationToken ct = default)
    {
        // Ensure reId is set
        request.ReId = string.IsNullOrEmpty(request.ReId) ? _options.ReId : request.ReId;
        if (string.IsNullOrEmpty(request.Timestamp))
            request.Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        var responseJson = await PostAsync("/api/ckyc/v1/download/validate", request, ct);
        return JsonSerializer.Deserialize<object>(responseJson, JsonOpts) ?? new { };
    }

    public async Task<OtpValidateResponse> ValidateOtpAsync(string ckycType, string downloadRefId, ValidateOtpPlain otpPayload, string? requestId = null, CancellationToken ct = default)
    {
        var encrypted = await _encryption.EncryptAsync(otpPayload, ct);

        var envelope = new
        {
            reId = _options.ReId,
            requestId = requestId ?? Guid.NewGuid().ToString(),
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            downloadRefId,
            ckycInq = new
            {
                ckycType,
                encryptedData = encrypted
            }
        };

        var responseJson = await PostAsync("/api/ckyc/v1/download/validate-otp", envelope, ct);
        return JsonSerializer.Deserialize<OtpValidateResponse>(responseJson, JsonOpts)
               ?? new OtpValidateResponse();
    }

    public async Task<object> ResendOtpAsync(string ckycType, string downloadRefId, string? requestId = null, CancellationToken ct = default)
    {
        var envelope = new
        {
            reId = _options.ReId,
            requestId = requestId ?? Guid.NewGuid().ToString(),
            downloadRefId,
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            ckycInq = new { ckycType }
        };

        var responseJson = await PostAsync("/api/ckyc/v1/download/resend-otp", envelope, ct);
        return JsonSerializer.Deserialize<object>(responseJson, JsonOpts) ?? new { };
    }

    public async Task<object> DownloadGetAsync(DownloadGetRequest request, CancellationToken ct = default)
    {
        request.ReId = string.IsNullOrEmpty(request.ReId) ? _options.ReId : request.ReId;
        if (string.IsNullOrEmpty(request.Timestamp))
            request.Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        var responseJson = await PostAsync("/api/ckyc/v1/download/get", request, ct);
        return JsonSerializer.Deserialize<object>(responseJson, JsonOpts) ?? new { };
    }

    // ----------------- Create / Update -----------------

    public async Task<CreateResponse> CreateAsync(string ckycType, object plainCreatePayload, string? requestId = null, CancellationToken ct = default)
    {
        var encrypted = await _encryption.EncryptAsync(plainCreatePayload, ct);

        var envelope = new CreateRequestEnvelope
        {
            ReId = _options.ReId,
            RequestId = requestId ?? Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            CkycInq = new CreateInquiry
            {
                CkycType = ckycType,
                Data = encrypted
            }
        };

        var responseJson = await PostAsync("/api/ckyc/v1/create", envelope, ct);
        return JsonSerializer.Deserialize<CreateResponse>(responseJson, JsonOpts)
               ?? new CreateResponse();
    }

    public async Task<CreateResponse> UpdateAsync(string ckycType, object plainUpdatePayload, string? requestId = null, CancellationToken ct = default)
    {
        var encrypted = await _encryption.EncryptAsync(plainUpdatePayload, ct);

        var envelope = new
        {
            reId = _options.ReId,
            requestId = requestId ?? Guid.NewGuid().ToString(),
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            ckycInq = new
            {
                ckycType,
                encryptedData = encrypted
            }
        };

        var responseJson = await PostAsync("/api/ckyc/v1/update", envelope, ct);
        return JsonSerializer.Deserialize<CreateResponse>(responseJson, JsonOpts)
               ?? new CreateResponse();
    }

    public async Task<object> GetUpdateStatusAsync(string ackNo, CancellationToken ct = default)
    {
        var path = $"/api/ckyc/v1/update/status?ackNo={Uri.EscapeDataString(ackNo)}";
        var responseJson = await GetAsync(path, ct);
        return JsonSerializer.Deserialize<object>(responseJson, JsonOpts) ?? new { };
    }

    public async Task<object> FetchUpdatesAsync(string lastUpdatedAfter, int? limit = null, int? page = null, CancellationToken ct = default)
    {
        var qs = $"lastUpdatedAfter={Uri.EscapeDataString(lastUpdatedAfter)}";
        if (limit.HasValue) qs += $"&limit={limit.Value}";
        if (page.HasValue) qs += $"&page={page.Value}";

        var responseJson = await GetAsync($"/api/ckyc/v1/update/fetch?{qs}", ct);
        return JsonSerializer.Deserialize<object>(responseJson, JsonOpts) ?? new { };
    }

    // ----------------- HTTP helpers -----------------

    private async Task<string> PostAsync(string relativePath, object body, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("CersaiMtls");
        var token = await _tokenService.GetAccessTokenAsync(ct);

        var request = new HttpRequestMessage(HttpMethod.Post, relativePath)
        {
            Content = new StringContent(JsonSerializer.Serialize(body, JsonOpts), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.TryAddWithoutValidation("x-correlation-id", Guid.NewGuid().ToString());
        // Idempotency key for mutation endpoints
        if (relativePath.Contains("create", StringComparison.OrdinalIgnoreCase) ||
            relativePath.Contains("update", StringComparison.OrdinalIgnoreCase))
        {
            request.Headers.TryAddWithoutValidation("x-idempotency-key", Guid.NewGuid().ToString());
        }

        _logger.LogDebug("POST {Path}", relativePath);

        using var response = await client.SendAsync(request, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("CERSAI call failed {Status}: {Body}", response.StatusCode, responseBody);
            throw new HttpRequestException($"CERSAI API error {(int)response.StatusCode}: {responseBody}");
        }

        return responseBody;
    }

    private async Task<string> GetAsync(string relativePath, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("CersaiMtls");
        var token = await _tokenService.GetAccessTokenAsync(ct);

        var request = new HttpRequestMessage(HttpMethod.Get, relativePath);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.TryAddWithoutValidation("x-correlation-id", Guid.NewGuid().ToString());

        using var response = await client.SendAsync(request, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("CERSAI GET failed {Status}: {Body}", response.StatusCode, responseBody);
            throw new HttpRequestException($"CERSAI API error {(int)response.StatusCode}: {responseBody}");
        }

        return responseBody;
    }
}
