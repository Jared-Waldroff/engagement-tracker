using EngagementTracker.Api.Extensions;
using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EngagementTracker.Api.Controllers;

/// <summary>
/// Manages client engagements. All endpoints require authentication.
/// Access is filtered by the authenticated user's role:
/// Partners see all, Managers see their engagements, Associates see engagements they've logged time against.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EngagementsController : ControllerBase
{
    private readonly IEngagementService _engagementService;
    private readonly ILogger<EngagementsController> _logger;

    public EngagementsController(IEngagementService engagementService, ILogger<EngagementsController> logger)
    {
        _engagementService = engagementService;
        _logger = logger;
    }

    /// <summary>
    /// Lists engagements visible to the authenticated user.
    /// Supports optional filtering by status, search text, and date range.
    /// </summary>
    /// <param name="filters">Optional query parameters for filtering.</param>
    /// <response code="200">Returns the list of engagement summaries.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<EngagementSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEngagements([FromQuery] EngagementFilterDto? filters)
    {
        var (userId, role) = HttpContext.GetUserClaims();
        _logger.LogInformation("User {UserId} ({Role}) requesting engagements", userId, role);

        var engagements = await _engagementService.GetEngagementsAsync(userId, role, filters);
        return Ok(engagements);
    }

    /// <summary>
    /// Returns detailed information about a specific engagement including
    /// time entries, per-user breakdowns, and budget metrics.
    /// </summary>
    /// <param name="id">The engagement ID.</param>
    /// <response code="200">Returns the engagement detail.</response>
    /// <response code="403">User does not have access to this engagement.</response>
    /// <response code="404">Engagement not found.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EngagementDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEngagement(int id)
    {
        var (userId, role) = HttpContext.GetUserClaims();
        var detail = await _engagementService.GetEngagementDetailAsync(id, userId, role);
        return Ok(detail);
    }

    /// <summary>
    /// Creates a new engagement. Only Managers and Partners can create engagements.
    /// The authenticated manager is automatically assigned as the engagement manager.
    /// </summary>
    /// <param name="dto">Engagement creation data.</param>
    /// <response code="201">Returns the created engagement summary.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="403">User role does not allow creating engagements.</response>
    [HttpPost]
    [ProducesResponseType(typeof(EngagementSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEngagement([FromBody] CreateEngagementDto dto)
    {
        var (userId, role) = HttpContext.GetUserClaims();

        if (role == Core.Enums.UserRole.Associate)
        {
            return Forbid();
        }

        var result = await _engagementService.CreateEngagementAsync(dto, userId);
        return CreatedAtAction(nameof(GetEngagement), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing engagement. Only the assigned Manager or a Partner can update.
    /// </summary>
    /// <param name="id">The engagement ID to update.</param>
    /// <param name="dto">The fields to update (all optional).</param>
    /// <response code="200">Returns the updated engagement summary.</response>
    /// <response code="403">User does not have permission to update.</response>
    /// <response code="404">Engagement not found.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EngagementSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEngagement(int id, [FromBody] UpdateEngagementDto dto)
    {
        var (userId, role) = HttpContext.GetUserClaims();
        var result = await _engagementService.UpdateEngagementAsync(id, dto, userId, role);
        return Ok(result);
    }

    /// <summary>
    /// Returns dashboard statistics tailored to the authenticated user's role.
    /// Includes total engagements, budget metrics, and top active engagements.
    /// </summary>
    /// <response code="200">Returns dashboard data.</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        var (userId, role) = HttpContext.GetUserClaims();
        var dashboard = await _engagementService.GetDashboardAsync(userId, role);
        return Ok(dashboard);
    }
}
