using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;
using EngagementTracker.Core.Exceptions;
using EngagementTracker.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace EngagementTracker.Core.Services;

/// <summary>
/// Implements time entry management. Associates log time against engagements
/// they have access to. The service validates engagement existence before creating entries.
/// </summary>
public class TimeEntryService : ITimeEntryService
{
    private readonly ITimeEntryRepository _repository;
    private readonly ILogger<TimeEntryService> _logger;

    public TimeEntryService(ITimeEntryRepository repository, ILogger<TimeEntryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<TimeEntryDto>> GetTimeEntriesAsync(
        int userId, UserRole role, int? engagementId = null)
    {
        _logger.LogInformation(
            "Fetching time entries for user {UserId} ({Role}), engagement filter: {EngagementId}",
            userId, role, engagementId?.ToString() ?? "all");

        return await _repository.GetTimeEntriesAsync(userId, role, engagementId);
    }

    /// <inheritdoc />
    public async Task<TimeEntryDto> CreateTimeEntryAsync(CreateTimeEntryDto dto, int userId)
    {
        bool engagementExists = await _repository.EngagementExistsAsync(dto.EngagementId);
        if (!engagementExists)
        {
            throw new NotFoundException(
                $"Engagement with ID {dto.EngagementId} was not found.",
                "ENGAGEMENT_NOT_FOUND");
        }

        _logger.LogInformation(
            "User {UserId} logging {Hours}h against engagement {EngagementId} for {Date:yyyy-MM-dd}",
            userId, dto.Hours, dto.EngagementId, dto.Date);

        var result = await _repository.CreateAsync(dto, userId);

        _logger.LogInformation("Time entry {TimeEntryId} created successfully", result.Id);
        return result;
    }
}
