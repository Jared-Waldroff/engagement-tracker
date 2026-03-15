using EngagementTracker.Core.Dtos;
using EngagementTracker.Core.Enums;
using EngagementTracker.Core.Exceptions;
using EngagementTracker.Core.Interfaces;
using EngagementTracker.Core.Services;
using EngagementTracker.Infrastructure.Data;
using EngagementTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace EngagementTracker.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the engagement repository.
/// Handles role-based query filtering and budget metric computation.
/// </summary>
public class EngagementRepository : IEngagementRepository
{
    private readonly AppDbContext _context;

    public EngagementRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<EngagementSummaryDto>> GetEngagementsAsync(
        int userId, UserRole role, EngagementFilterDto? filters)
    {
        IQueryable<Engagement> query = _context.Engagements
            .Include(e => e.Client)
            .Include(e => e.Manager)
            .Include(e => e.TimeEntries)
            .AsNoTracking();

        // Role-based filtering
        query = role switch
        {
            UserRole.Partner => query,
            UserRole.Manager => query.Where(e => e.ManagerId == userId),
            UserRole.Associate => query.Where(e => e.TimeEntries.Any(t => t.UserId == userId)),
            _ => query
        };

        // Apply optional filters
        if (filters != null)
        {
            if (!string.IsNullOrWhiteSpace(filters.Status) &&
                Enum.TryParse<EngagementStatus>(filters.Status, true, out var status))
            {
                query = query.Where(e => e.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                string search = filters.Search.ToLower();
                query = query.Where(e =>
                    e.Title.ToLower().Contains(search) ||
                    e.Client.Name.ToLower().Contains(search));
            }

            if (filters.StartDateFrom.HasValue)
            {
                query = query.Where(e => e.StartDate >= filters.StartDateFrom.Value);
            }

            if (filters.StartDateTo.HasValue)
            {
                query = query.Where(e => e.StartDate <= filters.StartDateTo.Value);
            }
        }

        var engagements = await query
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();

        return engagements.Select(ToSummaryDto).ToList();
    }

    /// <inheritdoc />
    public async Task<EngagementDetailDto?> GetEngagementDetailAsync(int engagementId)
    {
        var engagement = await _context.Engagements
            .Include(e => e.Client)
            .Include(e => e.Manager)
            .Include(e => e.TimeEntries)
                .ThenInclude(t => t.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == engagementId);

        if (engagement == null) return null;

        decimal hoursLogged = engagement.TimeEntries.Sum(t => t.Hours);
        decimal utilization = BudgetCalculator.CalculateUtilization(hoursLogged, engagement.BudgetHours);

        var recentEntries = engagement.TimeEntries
            .OrderByDescending(t => t.Date)
            .Take(20)
            .Select(t => new TimeEntryDto(
                t.Id,
                t.EngagementId,
                engagement.Title,
                $"{t.User.FirstName} {t.User.LastName}",
                t.Hours,
                t.Date,
                t.Description))
            .ToList();

        var hoursByUser = engagement.TimeEntries
            .GroupBy(t => new { t.UserId, t.User.FirstName, t.User.LastName, t.User.Role })
            .Select(g => new UserHoursSummaryDto(
                g.Key.UserId,
                $"{g.Key.FirstName} {g.Key.LastName}",
                g.Key.Role.ToString(),
                g.Sum(t => t.Hours)))
            .OrderByDescending(u => u.TotalHours)
            .ToList();

        return new EngagementDetailDto(
            engagement.Id,
            engagement.Client.Name,
            engagement.Client.Industry,
            engagement.Title,
            engagement.Description,
            engagement.Status.ToString(),
            engagement.BudgetHours,
            engagement.HourlyRate,
            hoursLogged,
            utilization,
            BudgetCalculator.DetermineBudgetStatus(utilization),
            engagement.BudgetHours * engagement.HourlyRate,
            hoursLogged * engagement.HourlyRate,
            $"{engagement.Manager.FirstName} {engagement.Manager.LastName}",
            engagement.StartDate,
            engagement.EndDate,
            recentEntries,
            hoursByUser
        );
    }

    /// <inheritdoc />
    public async Task<bool> UserHasAccessAsync(int engagementId, int userId, UserRole role)
    {
        return role switch
        {
            UserRole.Partner => await _context.Engagements.AnyAsync(e => e.Id == engagementId),
            UserRole.Manager => await _context.Engagements.AnyAsync(e => e.Id == engagementId && e.ManagerId == userId),
            UserRole.Associate => await _context.TimeEntries.AnyAsync(t => t.EngagementId == engagementId && t.UserId == userId),
            _ => false
        };
    }

    /// <inheritdoc />
    public async Task<EngagementSummaryDto> CreateAsync(CreateEngagementDto dto, int managerId)
    {
        var client = await _context.Clients.FindAsync(dto.ClientId)
            ?? throw new NotFoundException($"Client with ID {dto.ClientId} not found.", "CLIENT_NOT_FOUND");

        var engagement = new Engagement
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = EngagementStatus.Planning,
            BudgetHours = dto.BudgetHours,
            HourlyRate = dto.HourlyRate,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ClientId = dto.ClientId,
            ManagerId = managerId
        };

        _context.Engagements.Add(engagement);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        await _context.Entry(engagement).Reference(e => e.Client).LoadAsync();
        await _context.Entry(engagement).Reference(e => e.Manager).LoadAsync();

        return ToSummaryDto(engagement);
    }

    /// <inheritdoc />
    public async Task<EngagementSummaryDto> UpdateAsync(int engagementId, UpdateEngagementDto dto)
    {
        var engagement = await _context.Engagements
            .Include(e => e.Client)
            .Include(e => e.Manager)
            .Include(e => e.TimeEntries)
            .FirstOrDefaultAsync(e => e.Id == engagementId)
            ?? throw new NotFoundException(
                $"Engagement with ID {engagementId} not found.",
                "ENGAGEMENT_NOT_FOUND");

        if (dto.Title != null) engagement.Title = dto.Title;
        if (dto.Description != null) engagement.Description = dto.Description;
        if (dto.BudgetHours.HasValue) engagement.BudgetHours = dto.BudgetHours.Value;
        if (dto.HourlyRate.HasValue) engagement.HourlyRate = dto.HourlyRate.Value;
        if (dto.EndDate.HasValue) engagement.EndDate = dto.EndDate.Value;

        if (dto.Status != null && Enum.TryParse<EngagementStatus>(dto.Status, true, out var status))
        {
            engagement.Status = status;
        }

        await _context.SaveChangesAsync();
        return ToSummaryDto(engagement);
    }

    /// <inheritdoc />
    public async Task<DashboardDto> GetDashboardDataAsync(int userId, UserRole role)
    {
        IQueryable<Engagement> query = _context.Engagements
            .Include(e => e.Client)
            .Include(e => e.Manager)
            .Include(e => e.TimeEntries)
            .AsNoTracking();

        query = role switch
        {
            UserRole.Partner => query,
            UserRole.Manager => query.Where(e => e.ManagerId == userId),
            UserRole.Associate => query.Where(e => e.TimeEntries.Any(t => t.UserId == userId)),
            _ => query
        };

        var engagements = await query.ToListAsync();
        var summaries = engagements.Select(ToSummaryDto).ToList();

        decimal totalHoursLogged = summaries.Sum(s => s.HoursLogged);
        decimal totalBudgetHours = summaries.Sum(s => s.BudgetHours);

        return new DashboardDto(
            TotalEngagements: summaries.Count,
            ActiveEngagements: summaries.Count(s => s.Status == "Active"),
            TotalHoursLogged: totalHoursLogged,
            TotalBudgetHours: totalBudgetHours,
            OverallUtilizationPercent: BudgetCalculator.CalculateUtilization(totalHoursLogged, totalBudgetHours),
            EngagementsOnTrack: summaries.Count(s => s.BudgetStatus == "OnTrack"),
            EngagementsAtRisk: summaries.Count(s => s.BudgetStatus == "AtRisk"),
            EngagementsOverBudget: summaries.Count(s => s.BudgetStatus == "OverBudget"),
            TopEngagements: summaries
                .Where(s => s.Status == "Active")
                .OrderByDescending(s => s.BudgetUtilizationPercent)
                .Take(5)
                .ToList()
        );
    }

    private static EngagementSummaryDto ToSummaryDto(Engagement engagement)
    {
        decimal hoursLogged = engagement.TimeEntries?.Sum(t => t.Hours) ?? 0;
        decimal utilization = BudgetCalculator.CalculateUtilization(hoursLogged, engagement.BudgetHours);

        return new EngagementSummaryDto(
            engagement.Id,
            engagement.Client?.Name ?? "Unknown",
            engagement.Title,
            engagement.Status.ToString(),
            engagement.BudgetHours,
            hoursLogged,
            utilization,
            BudgetCalculator.DetermineBudgetStatus(utilization),
            engagement.Manager != null ? $"{engagement.Manager.FirstName} {engagement.Manager.LastName}" : "Unassigned",
            engagement.StartDate,
            engagement.EndDate
        );
    }
}
