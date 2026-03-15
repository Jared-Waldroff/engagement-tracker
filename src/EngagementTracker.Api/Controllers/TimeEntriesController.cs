using EngagementTracker.Api.Extensions;
using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EngagementTracker.Api.Controllers;

/// <summary>
/// Manages time entries. Associates log hours against engagements.
/// All endpoints require authentication. Results are filtered by role.
/// </summary>
[ApiController]
[Route("api/time-entries")]
[Authorize]
[Produces("application/json")]
public class TimeEntriesController : ControllerBase
{
    private readonly ITimeEntryService _timeEntryService;
    private readonly ILogger<TimeEntriesController> _logger;

    public TimeEntriesController(ITimeEntryService timeEntryService, ILogger<TimeEntriesController> logger)
    {
        _timeEntryService = timeEntryService;
        _logger = logger;
    }

    /// <summary>
    /// Lists time entries visible to the authenticated user.
    /// Associates see their own entries. Managers see entries for their engagements.
    /// Partners see all entries. Optionally filter by engagement ID.
    /// </summary>
    /// <param name="engagementId">Optional engagement ID to filter by.</param>
    /// <response code="200">Returns the list of time entries.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TimeEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTimeEntries([FromQuery] int? engagementId)
    {
        var (userId, role) = HttpContext.GetUserClaims();
        var entries = await _timeEntryService.GetTimeEntriesAsync(userId, role, engagementId);
        return Ok(entries);
    }

    /// <summary>
    /// Logs a new time entry for the authenticated user.
    /// The engagement must exist and hours must be between 0.25 and 24.
    /// </summary>
    /// <param name="dto">Time entry data.</param>
    /// <response code="201">Returns the created time entry.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="404">Engagement not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TimeEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTimeEntry([FromBody] CreateTimeEntryDto dto)
    {
        var (userId, _) = HttpContext.GetUserClaims();
        var result = await _timeEntryService.CreateTimeEntryAsync(dto, userId);
        return CreatedAtAction(nameof(GetTimeEntries), null, result);
    }
}
