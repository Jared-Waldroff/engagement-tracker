namespace EngagementTracker.Core.Dtos;

/// <summary>
/// Request body for user login.
/// </summary>
public record LoginRequestDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Response body returned on successful authentication.
/// Contains the JWT access token and user profile information.
/// </summary>
public record LoginResponseDto(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    UserProfileDto User
);

/// <summary>
/// Basic user profile information included in auth responses.
/// </summary>
public record UserProfileDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string Role
);

/// <summary>
/// Dashboard statistics tailored to the user's role.
/// </summary>
public record DashboardDto(
    int TotalEngagements,
    int ActiveEngagements,
    decimal TotalHoursLogged,
    decimal TotalBudgetHours,
    decimal OverallUtilizationPercent,
    int EngagementsOnTrack,
    int EngagementsAtRisk,
    int EngagementsOverBudget,
    List<EngagementSummaryDto> TopEngagements
);
