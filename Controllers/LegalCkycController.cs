using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CersaiCkycApi.Models.Common;
using CersaiCkycApi.Models.Download;
using CersaiCkycApi.Models.Search;
using CersaiCkycApi.Services;

namespace CersaiCkycApi.Controllers;

/// <summary>
/// Endpoints for Legal (entity / non-individual) KYC operations against CERSAI CKYCRR 2.0.
/// All outbound calls use mTLS + OAuth Bearer token.
/// </summary>
[Authorize]
[ApiController]
[Route("api/legal")]
public class LegalCkycController : ControllerBase
{
    private readonly ICkycClient _client;
    private readonly ILogger<LegalCkycController> _logger;

    public LegalCkycController(ICkycClient client, ILogger<LegalCkycController> logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// Search Legal entity KYC records.
    /// Supported searchOption values: OVD | MOBILE | DOI | CKYC
    /// </summary>
    [HttpPost("search/{searchOption}")]
    public async Task<IActionResult> Search(
        [FromRoute] string searchOption,
        [FromBody] object payload,
        CancellationToken ct)
    {
        var allowed = new[]
        {
            SearchOptionLegal.Ovd,
            SearchOptionLegal.Mobile,
            SearchOptionLegal.Doi,
            SearchOptionLegal.Ckyc
        };

        if (!allowed.Contains(searchOption, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { message = $"Invalid searchOption. Allowed: {string.Join(", ", allowed)}" });

        try
        {
            var result = await _client.SearchLegalAsync(searchOption.ToUpperInvariant(), payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Legal search failed");
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Initiate download of a Legal entity KYC record.
    /// Provide exactly one of: ckycNo | ckycRefNo | ckycAlias.
    /// For Legal entities OTP is triggered automatically on initiate (no separate consent type step).
    /// </summary>
    [HttpPost("download/initiate")]
    public async Task<IActionResult> DownloadInitiate([FromBody] DownloadInitiatePlain payload, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(payload.CkycNo) &&
            string.IsNullOrEmpty(payload.CkycRefNo) &&
            string.IsNullOrEmpty(payload.CkycAlias))
        {
            return BadRequest(new { message = "Provide exactly one of ckycNo, ckycRefNo or ckycAlias" });
        }

        try
        {
            var result = await _client.DownloadInitiateAsync(CkycType.Legal, payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Validate OTP for Legal entity download (Legal uses OTP only).
    /// </summary>
    [HttpPost("download/validate-otp")]
    public async Task<IActionResult> ValidateOtp(
        [FromQuery] string downloadRefId,
        [FromBody] ValidateOtpPlain payload,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(downloadRefId))
            return BadRequest(new { message = "downloadRefId is required" });

        try
        {
            var result = await _client.ValidateOtpAsync(CkycType.Legal, downloadRefId, payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Resend OTP for Legal entity download.
    /// </summary>
    [HttpPost("download/resend-otp")]
    public async Task<IActionResult> ResendOtp([FromQuery] string downloadRefId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(downloadRefId))
            return BadRequest(new { message = "downloadRefId is required" });

        try
        {
            var result = await _client.ResendOtpAsync(CkycType.Legal, downloadRefId, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieve the full Legal entity KYC record after successful OTP.
    /// </summary>
    [HttpPost("download/get")]
    public async Task<IActionResult> DownloadGet([FromBody] DownloadGetRequest request, CancellationToken ct)
    {
        request.CkycInq.CkycType = CkycType.Legal;
        try
        {
            var result = await _client.DownloadGetAsync(request, ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new Legal entity KYC record.
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] object payload, CancellationToken ct)
    {
        try
        {
            var result = await _client.CreateAsync(CkycType.Legal, payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing Legal entity KYC record.
    /// </summary>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] object payload, CancellationToken ct)
    {
        try
        {
            var result = await _client.UpdateAsync(CkycType.Legal, payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }
}
