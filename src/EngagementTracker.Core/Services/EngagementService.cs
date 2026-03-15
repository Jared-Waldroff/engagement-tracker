using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;
using EngagementTracker.Core.Exceptions;
using EngagementTracker.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace EngagementTracker.Core.Services;

/// <summary>
/// Implements engagement management with role-based access control.
/// Partners see all engagements. Managers see engagements they manage.
/// Associates see engagements they have time entries for.
/// </summary>
public class EngagementService : IEngagementService
{
    private readonly IEngagementRepository _repository;
    private readonly ILogger<EngagementService> _logger;

    public EngagementService(IEngagementRepository repository, ILogger<EngagementService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<EngagementSummaryDto>> GetEngagementsAsync(
        int userId, UserRole role, EngagementFilterDto? filters = null)
    {
        _logger.LogInformation(
            "Fetching engagements for user {UserId} ({Role}) with filters {@Filters}",
            userId, role, filters);

        return await _repository.GetEngagementsAsync(userId, role, filters);
    }

    /// <inheritdoc />
    public async Task<EngagementDetailDto> GetEngagementDetailAsync(
        int engagementId, int userId, UserRole role)
    {
        bool hasAccess = await _repository.UserHasAccessAsync(engagementId, userId, role);
        if (!hasAccess)
        {
            var detail = await _repository.GetEngagementDetailAsync(engagementId);
            if (detail == null)
            {
                throw new NotFoundException(
                    $"Engagement with ID {engagementId} was not found.",
                    "ENGAGEMENT_NOT_FOUND");
            }

            throw new ForbiddenException(
                $"You do not have access to engagement {engagementId}.",
                "ENGAGEMENT_ACCESS_DENIED");
        }

        var engagement = await _repository.GetEngagementDetailAsync(engagementId)
            ?? throw new NotFoundException(
                $"Engagement with ID {engagementId} was not found.",
                "ENGAGEMENT_NOT_FOUND");

        _logger.LogInformation(
            "User {UserId} accessed engagement {EngagementId} detail",
            userId, engagementId);

        return engagement;
    }

    /// <inheritdoc />
    public async Task<EngagementSummaryDto> CreateEngagementAsync(
        CreateEngagementDto dto, int managerId)
    {
        _logger.LogInformation(
            "Manager {ManagerId} creating engagement '{Title}' for client {ClientId}",
            managerId, dto.Title, dto.ClientId);

        var result = await _repository.CreateAsync(dto, managerId);

        _logger.LogInformation("Engagement {EngagementId} created successfully", result.Id);
        return result;
    }

    /// <inheritdoc />
    public async Task<EngagementSummaryDto> UpdateEngagementAsync(
        int engagementId, UpdateEngagementDto dto, int userId, UserRole role)
    {
        bool hasAccess = await _repository.UserHasAccessAsync(engagementId, userId, role);
        if (!hasAccess)
        {
            throw new ForbiddenException(
                $"You do not have permission to update engagement {engagementId}.",
                "ENGAGEMENT_UPDATE_DENIED");
        }

        // Only Managers and Partners can update engagements
        if (role == UserRole.Associate)
        {
            throw new ForbiddenException(
                "Associates cannot update engagements.",
                "INSUFFICIENT_ROLE");
        }

        _logger.LogInformation(
            "User {UserId} updating engagement {EngagementId}",
            userId, engagementId);

        return await _repository.UpdateAsync(engagementId, dto);
    }

    /// <inheritdoc />
    public async Task<DashboardDto> GetDashboardAsync(int userId, UserRole role)
    {
        _logger.LogInformation(
            "Fetching dashboard for user {UserId} ({Role})", userId, role);

        return await _repository.GetDashboardDataAsync(userId, role);
    }
}
