using EngagementTracker.Api.Extensions;
using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EngagementTracker.Api.Controllers;

/// <summary>
/// Handles user authentication. Provides login and profile endpoints.
/// Login is public; profile requires a valid JWT token.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// Returns a JWT access token on success.
    /// </summary>
    /// <param name="request">Email and password credentials.</param>
    /// <response code="200">Returns the JWT token and user profile.</response>
    /// <response code="400">Validation failed (missing email or password).</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Returns the profile of the currently authenticated user.
    /// </summary>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile()
    {
        var (userId, _) = HttpContext.GetUserClaims();
        var profile = await _authService.GetProfileAsync(userId);
        return Ok(profile);
    }
}
