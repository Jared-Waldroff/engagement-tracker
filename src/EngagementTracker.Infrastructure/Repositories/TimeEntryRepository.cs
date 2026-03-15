using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;
using EngagementTracker.Core.Interfaces;
using EngagementTracker.Infrastructure.Data;
using EngagementTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace EngagementTracker.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the time entry repository.
/// Handles role-based query filtering for time entry access.
/// </summary>
public class TimeEntryRepository : ITimeEntryRepository
{
    private readonly AppDbContext _context;

    public TimeEntryRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<TimeEntryDto>> GetTimeEntriesAsync(
        int userId, UserRole role, int? engagementId)
    {
        IQueryable<TimeEntry> query = _context.TimeEntries
            .Include(t => t.User)
            .Include(t => t.Engagement)
            .AsNoTracking();

        // Role-based filtering
        query = role switch
        {
            UserRole.Partner => query,
            UserRole.Manager => query.Where(t => t.Engagement.ManagerId == userId),
            UserRole.Associate => query.Where(t => t.UserId == userId),
            _ => query
        };

        if (engagementId.HasValue)
        {
            query = query.Where(t => t.EngagementId == engagementId.Value);
        }

        var entries = await query
            .OrderByDescending(t => t.Date)
            .Take(100)
            .ToListAsync();

        return entries.Select(t => new TimeEntryDto(
            t.Id,
            t.EngagementId,
            t.Engagement.Title,
            $"{t.User.FirstName} {t.User.LastName}",
            t.Hours,
            t.Date,
            t.Description
        )).ToList();
    }

    /// <inheritdoc />
    public async Task<TimeEntryDto> CreateAsync(CreateTimeEntryDto dto, int userId)
    {
        var entry = new TimeEntry
        {
            EngagementId = dto.EngagementId,
            UserId = userId,
            Hours = dto.Hours,
            Date = dto.Date,
            Description = dto.Description
        };

        _context.TimeEntries.Add(entry);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        await _context.Entry(entry).Reference(e => e.User).LoadAsync();
        await _context.Entry(entry).Reference(e => e.Engagement).LoadAsync();

        return new TimeEntryDto(
            entry.Id,
            entry.EngagementId,
            entry.Engagement.Title,
            $"{entry.User.FirstName} {entry.User.LastName}",
            entry.Hours,
            entry.Date,
            entry.Description
        );
    }

    /// <inheritdoc />
    public async Task<bool> EngagementExistsAsync(int engagementId)
    {
        return await _context.Engagements.AnyAsync(e => e.Id == engagementId);
    }
}
