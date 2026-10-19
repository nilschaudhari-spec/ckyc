using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CersaiCkycApi.Models.Common;
using CersaiCkycApi.Models.Download;
using CersaiCkycApi.Models.Search;
using CersaiCkycApi.Services;

namespace CersaiCkycApi.Controllers;

/// <summary>
/// Endpoints for Individual (natural person) KYC operations against CERSAI CKYCRR 2.0.
/// All outbound calls use mTLS + OAuth Bearer token.
/// </summary>
[Authorize]
[ApiController]
[Route("api/individual")]
public class IndividualCkycController : ControllerBase
{
    private readonly ICkycClient _client;
    private readonly ILogger<IndividualCkycController> _logger;

    public IndividualCkycController(ICkycClient client, ILogger<IndividualCkycController> logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// Search Individual KYC records.
    /// Supported searchOption values: OVD | CKYC | DEMOGRAPHIC | PHOTO | MOBILE | VC
    /// Body = plain search payload (will be encrypted before calling CERSAI).
    /// </summary>
    [HttpPost("search/{searchOption}")]
    public async Task<IActionResult> Search(
        [FromRoute] string searchOption,
        [FromBody] object payload,
        CancellationToken ct)
    {
        var allowed = new[]
        {
            SearchOptionIndividual.Ovd,
            SearchOptionIndividual.Ckyc,
            SearchOptionIndividual.Demographic,
            SearchOptionIndividual.Photo,
            SearchOptionIndividual.Mobile,
            SearchOptionIndividual.Vc
        };

        if (!allowed.Contains(searchOption, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { message = $"Invalid searchOption. Allowed: {string.Join(", ", allowed)}" });

        try
        {
            var result = await _client.SearchIndividualAsync(searchOption.ToUpperInvariant(), payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Individual search failed");
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Initiate download of an Individual KYC record.
    /// Provide exactly one of: ckycNo | ckycRefNo | ckycAlias.
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
            var result = await _client.DownloadInitiateAsync(CkycType.Individual, payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Validate consent for download (OTP / PHYSICAL / FACEAUTH).
    /// For OTP flow encryptedData is not required.
    /// For PHYSICAL / FACEAUTH supply authFactors inside encryptedData.
    /// </summary>
    [HttpPost("download/validate")]
    public async Task<IActionResult> DownloadValidate([FromBody] DownloadValidateRequest request, CancellationToken ct)
    {
        request.CkycInq.CkycType = CkycType.Individual;
        try
        {
            var result = await _client.DownloadValidateAsync(request, ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Validate OTP received on mobile / email.
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
            var result = await _client.ValidateOtpAsync(CkycType.Individual, downloadRefId, payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Resend OTP for an ongoing download journey.
    /// </summary>
    [HttpPost("download/resend-otp")]
    public async Task<IActionResult> ResendOtp([FromQuery] string downloadRefId, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(downloadRefId))
            return BadRequest(new { message = "downloadRefId is required" });

        try
        {
            var result = await _client.ResendOtpAsync(CkycType.Individual, downloadRefId, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieve the full KYC record after successful consent / OTP.
    /// </summary>
    [HttpPost("download/get")]
    public async Task<IActionResult> DownloadGet([FromBody] DownloadGetRequest request, CancellationToken ct)
    {
        request.CkycInq.CkycType = CkycType.Individual;
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
    /// Create a new Individual KYC record.
    /// Body = plain IndividualCreatePayload (will be encrypted).
    /// searchKey from a prior Search call is mandatory and valid for 15 days.
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] object payload, CancellationToken ct)
    {
        try
        {
            var result = await _client.CreateAsync(CkycType.Individual, payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing Individual KYC record.
    /// Body must contain ckycNo plus only the fields that need to be changed.
    /// </summary>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] object payload, CancellationToken ct)
    {
        try
        {
            var result = await _client.UpdateAsync(CkycType.Individual, payload, ct: ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }
}
