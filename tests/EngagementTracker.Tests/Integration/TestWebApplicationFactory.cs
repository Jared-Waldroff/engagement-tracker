using EngagementTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EngagementTracker.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory that replaces MySQL with in-memory SQLite
/// for integration testing. Uses a shared connection to keep the in-memory
/// database alive across the entire test class lifetime.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public TestWebApplicationFactory()
    {
        // SQLite in-memory DB lives only as long as the connection is open
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real MySQL DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Also remove the DbSeeder to prevent MigrateAsync on SQLite
            var seederDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbSeeder));
            if (seederDescriptor != null) services.Remove(seederDescriptor);

            // Replace with SQLite using the shared connection
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));

            // Re-add DbSeeder so it's available
            services.AddScoped<DbSeeder>();

            // Suppress noisy EF Core logging in tests
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Warning);
            });
        });

        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // EnsureCreated instead of Migrate for SQLite in-memory
            db.Database.EnsureCreated();

            // Seed directly — bypass the seeder's MigrateAsync call
            SeedTestData(db);
        });

        builder.UseEnvironment("Development");
    }

    private static void SeedTestData(AppDbContext context)
    {
        if (context.Users.Any()) return;

        string passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");

        var alice = new Infrastructure.Entities.User
        {
            Email = "alice@example.com", PasswordHash = passwordHash,
            Role = Core.Enums.UserRole.Associate, FirstName = "Alice", LastName = "Chen"
        };
        var bob = new Infrastructure.Entities.User
        {
            Email = "bob@example.com", PasswordHash = passwordHash,
            Role = Core.Enums.UserRole.Manager, FirstName = "Bob", LastName = "Martinez"
        };
        var carol = new Infrastructure.Entities.User
        {
            Email = "carol@example.com", PasswordHash = passwordHash,
            Role = Core.Enums.UserRole.Partner, FirstName = "Carol", LastName = "Thompson"
        };

        context.Users.AddRange(alice, bob, carol);

        var client1 = new Infrastructure.Entities.Client { Name = "Apex Energy Corp", Industry = "Energy", ContactEmail = "f@a.com" };
        var client2 = new Infrastructure.Entities.Client { Name = "Northern Trust", Industry = "Financial Services", ContactEmail = "o@n.com" };
        context.Clients.AddRange(client1, client2);

        var eng1 = new Infrastructure.Entities.Engagement
        {
            Title = "Annual Financial Audit 2026", Description = "Audit", Status = Core.Enums.EngagementStatus.Active,
            BudgetHours = 200, HourlyRate = 175, StartDate = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc),
            Client = client1, Manager = bob
        };
        var eng2 = new Infrastructure.Entities.Engagement
        {
            Title = "Tax Advisory", Description = "Tax", Status = Core.Enums.EngagementStatus.Active,
            BudgetHours = 80, HourlyRate = 200, StartDate = new DateTime(2026, 1, 13, 0, 0, 0, DateTimeKind.Utc),
            Client = client2, Manager = bob
        };
        var eng3 = new Infrastructure.Entities.Engagement
        {
            Title = "Completed Project", Description = "Done", Status = Core.Enums.EngagementStatus.Completed,
            BudgetHours = 60, HourlyRate = 165, StartDate = new DateTime(2025, 12, 7, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc), Client = client1, Manager = bob
        };

        context.Engagements.AddRange(eng1, eng2, eng3);

        // Alice has time entries on eng1 and eng2 (not eng3)
        context.TimeEntries.AddRange(
            new Infrastructure.Entities.TimeEntry { User = alice, Engagement = eng1, Hours = 40, Date = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc), Description = "Audit work" },
            new Infrastructure.Entities.TimeEntry { User = alice, Engagement = eng2, Hours = 20, Date = new DateTime(2026, 1, 13, 0, 0, 0, DateTimeKind.Utc), Description = "Tax prep" },
            new Infrastructure.Entities.TimeEntry { User = bob, Engagement = eng1, Hours = 10, Date = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc), Description = "Review" }
        );

        context.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection.Dispose();
    }
}
