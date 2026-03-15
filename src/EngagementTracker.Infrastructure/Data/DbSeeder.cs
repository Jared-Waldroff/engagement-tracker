using EngagementTracker.Core.Enums;
using EngagementTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EngagementTracker.Infrastructure.Data;

/// <summary>
/// Seeds the database with realistic sample data on first run.
/// Creates demo users (one per role), clients across industries,
/// active engagements with budgets, and time entries spread across dates.
/// </summary>
public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(AppDbContext context, ILogger<DbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Applies pending migrations and seeds data if the database is empty.
    /// Safe to call on every startup — skips seeding if users already exist.
    /// </summary>
    public async Task SeedAsync()
    {
        if (_context.Database.IsSqlite())
        {
            await _context.Database.EnsureCreatedAsync();
        }
        else
        {
            await _context.Database.MigrateAsync();
        }

        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Database already seeded, skipping");
            return;
        }

        _logger.LogInformation("Seeding database with sample data");

        var users = CreateUsers();
        var clients = CreateClients();
        var engagements = CreateEngagements(users, clients);
        var timeEntries = CreateTimeEntries(users, engagements);

        _context.Users.AddRange(users);
        _context.Clients.AddRange(clients);
        _context.Engagements.AddRange(engagements);
        _context.TimeEntries.AddRange(timeEntries);

        await _context.SaveChangesAsync();
        _logger.LogInformation(
            "Seeded {Users} users, {Clients} clients, {Engagements} engagements, {TimeEntries} time entries",
            users.Count, clients.Count, engagements.Count, timeEntries.Count);
    }

    private static List<User> CreateUsers()
    {
        // BCrypt hash of "password123" — pre-computed for deterministic seeding
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");

        return new List<User>
        {
            new()
            {
                Email = "alice@example.com",
                PasswordHash = passwordHash,
                Role = UserRole.Associate,
                FirstName = "Alice",
                LastName = "Chen"
            },
            new()
            {
                Email = "bob@example.com",
                PasswordHash = passwordHash,
                Role = UserRole.Manager,
                FirstName = "Bob",
                LastName = "Martinez"
            },
            new()
            {
                Email = "carol@example.com",
                PasswordHash = passwordHash,
                Role = UserRole.Partner,
                FirstName = "Carol",
                LastName = "Thompson"
            }
        };
    }

    private static List<Client> CreateClients()
    {
        return new List<Client>
        {
            new() { Name = "Apex Energy Corp", Industry = "Energy", ContactEmail = "finance@apexenergy.com" },
            new() { Name = "Northern Trust Financial", Industry = "Financial Services", ContactEmail = "ops@northerntrust.com" },
            new() { Name = "MedLine Health Systems", Industry = "Healthcare", ContactEmail = "admin@medlinehealth.com" },
            new() { Name = "Pinnacle Technologies", Industry = "Technology", ContactEmail = "cfo@pinnacletech.com" },
            new() { Name = "Harvest Retail Group", Industry = "Retail", ContactEmail = "accounting@harvestretail.com" }
        };
    }

    private static List<Engagement> CreateEngagements(List<User> users, List<Client> clients)
    {
        User manager = users[1]; // Bob (Manager)
        var baseDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

        return new List<Engagement>
        {
            new()
            {
                Title = "Annual Financial Audit 2026",
                Description = "Comprehensive audit of financial statements for regulatory compliance.",
                Status = EngagementStatus.Active,
                BudgetHours = 200,
                HourlyRate = 175,
                StartDate = baseDate,
                Client = clients[0], // Apex Energy
                Manager = manager
            },
            new()
            {
                Title = "Tax Advisory — Q1 Filing",
                Description = "Preparation and filing of quarterly tax returns with CRA optimization strategies.",
                Status = EngagementStatus.Active,
                BudgetHours = 80,
                HourlyRate = 200,
                StartDate = baseDate.AddDays(7),
                Client = clients[1], // Northern Trust
                Manager = manager
            },
            new()
            {
                Title = "IT Systems Risk Assessment",
                Description = "Security and compliance review of patient data management systems.",
                Status = EngagementStatus.Active,
                BudgetHours = 120,
                HourlyRate = 190,
                StartDate = baseDate.AddDays(14),
                Client = clients[2], // MedLine Health
                Manager = manager
            },
            new()
            {
                Title = "ERP Implementation Advisory",
                Description = "Advisory services for SAP S/4HANA migration planning and vendor evaluation.",
                Status = EngagementStatus.Planning,
                BudgetHours = 300,
                HourlyRate = 210,
                StartDate = baseDate.AddDays(60),
                Client = clients[3], // Pinnacle Tech
                Manager = manager
            },
            new()
            {
                Title = "Inventory Valuation Review",
                Description = "Year-end inventory count observation and valuation methodology assessment.",
                Status = EngagementStatus.Completed,
                BudgetHours = 60,
                HourlyRate = 165,
                StartDate = baseDate.AddDays(-30),
                EndDate = baseDate.AddDays(-2),
                Client = clients[4], // Harvest Retail
                Manager = manager
            },
            new()
            {
                Title = "Internal Controls Assessment",
                Description = "Evaluation of internal controls over financial reporting (ICFR) for SOX compliance.",
                Status = EngagementStatus.Active,
                BudgetHours = 150,
                HourlyRate = 185,
                StartDate = baseDate.AddDays(21),
                Client = clients[0], // Apex Energy
                Manager = manager
            },
            new()
            {
                Title = "Restructuring Advisory",
                Description = "Financial modeling and advisory for corporate restructuring under CCAA.",
                Status = EngagementStatus.OnHold,
                BudgetHours = 250,
                HourlyRate = 225,
                StartDate = baseDate.AddDays(30),
                Client = clients[1], // Northern Trust
                Manager = manager
            },
            new()
            {
                Title = "Data Analytics Transformation",
                Description = "Design and implementation of business intelligence dashboards for retail operations.",
                Status = EngagementStatus.Active,
                BudgetHours = 100,
                HourlyRate = 195,
                StartDate = baseDate.AddDays(28),
                Client = clients[4], // Harvest Retail
                Manager = manager
            }
        };
    }

    private static List<TimeEntry> CreateTimeEntries(List<User> users, List<Engagement> engagements)
    {
        User alice = users[0]; // Associate
        User bob = users[1];   // Manager
        var entries = new List<TimeEntry>();
        var baseDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc);

        // Engagement 0: Annual Financial Audit — heavy hours, approaching budget
        // 200 budget hours, we'll log ~170 hours (85% utilization = AtRisk)
        AddWeeklyEntries(entries, alice, engagements[0], baseDate, 8, 7.5m, "Audit fieldwork — testing revenue recognition controls");
        AddWeeklyEntries(entries, alice, engagements[0], baseDate, 8, 4.0m, "Substantive testing of accounts receivable");
        AddWeeklyEntries(entries, bob, engagements[0], baseDate, 8, 2.5m, "Manager review of audit workpapers");

        // Engagement 1: Tax Advisory — over budget
        // 80 budget hours, we'll log ~90 hours (112% utilization = OverBudget)
        AddWeeklyEntries(entries, alice, engagements[1], baseDate.AddDays(7), 7, 8.0m, "Tax return preparation and documentation");
        AddWeeklyEntries(entries, bob, engagements[1], baseDate.AddDays(7), 7, 3.0m, "Tax strategy review and CRA correspondence");

        // Engagement 2: IT Risk Assessment — early stage
        // 120 budget hours, we'll log ~30 hours (25% utilization = OnTrack)
        AddWeeklyEntries(entries, alice, engagements[2], baseDate.AddDays(14), 4, 6.0m, "Security controls documentation and testing");
        AddWeeklyEntries(entries, bob, engagements[2], baseDate.AddDays(14), 2, 2.0m, "Risk assessment framework review");

        // Engagement 4: Completed — exactly at budget
        // 60 budget hours, log 58 hours
        AddWeeklyEntries(entries, alice, engagements[4], baseDate.AddDays(-30), 4, 10.0m, "Inventory count observation and sample testing");
        AddWeeklyEntries(entries, bob, engagements[4], baseDate.AddDays(-30), 4, 3.5m, "Valuation methodology review");

        // Engagement 5: Internal Controls — moderate progress
        // 150 budget hours, ~45 hours (30% utilization = OnTrack)
        AddWeeklyEntries(entries, alice, engagements[5], baseDate.AddDays(21), 5, 6.5m, "Walkthrough testing of key controls");
        AddWeeklyEntries(entries, bob, engagements[5], baseDate.AddDays(21), 3, 2.0m, "Control deficiency evaluation");

        // Engagement 7: Data Analytics — moderate progress
        // 100 budget hours, ~35 hours (35% utilization = OnTrack)
        AddWeeklyEntries(entries, alice, engagements[7], baseDate.AddDays(28), 4, 7.0m, "Dashboard requirements gathering and data mapping");

        return entries;
    }

    /// <summary>
    /// Helper to generate time entries spread across weekdays.
    /// Creates one entry per week for the specified number of weeks.
    /// </summary>
    private static void AddWeeklyEntries(
        List<TimeEntry> entries,
        User user,
        Engagement engagement,
        DateTime startDate,
        int weeks,
        decimal hoursPerWeek,
        string description)
    {
        for (int week = 0; week < weeks; week++)
        {
            entries.Add(new TimeEntry
            {
                User = user,
                Engagement = engagement,
                Hours = hoursPerWeek,
                Date = startDate.AddDays(week * 7),
                Description = description
            });
        }
    }
}
