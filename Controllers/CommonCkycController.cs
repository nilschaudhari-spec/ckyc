using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CersaiCkycApi.Services;

namespace CersaiCkycApi.Controllers;

/// <summary>
/// Shared endpoints that are not entity-type specific.
/// </summary>
[Authorize]
[ApiController]
[Route("api/ckyc")]
public class CommonCkycController : ControllerBase
{
    private readonly ICkycClient _client;

    public CommonCkycController(ICkycClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Check status of a Create / Update submission using the acknowledgment number.
    /// </summary>
    [HttpGet("update/status")]
    public async Task<IActionResult> GetUpdateStatus([FromQuery] string ackNo, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ackNo))
            return BadRequest(new { message = "ackNo is required" });

        try
        {
            var result = await _client.GetUpdateStatusAsync(ackNo, ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Fetch list of linked CKYC numbers that have been updated after a given date.
    /// lastUpdatedAfter format: DD-MM-YYYY
    /// </summary>
    [HttpGet("update/fetch")]
    public async Task<IActionResult> FetchUpdates(
        [FromQuery] string lastUpdatedAfter,
        [FromQuery] int? limit,
        [FromQuery] int? page,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(lastUpdatedAfter))
            return BadRequest(new { message = "lastUpdatedAfter (DD-MM-YYYY) is required" });

        try
        {
            var result = await _client.FetchUpdatesAsync(lastUpdatedAfter, limit, page, ct);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new { message = ex.Message });
        }
    }
}
