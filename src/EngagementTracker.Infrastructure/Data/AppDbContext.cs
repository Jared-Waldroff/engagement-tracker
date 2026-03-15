using EngagementTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace EngagementTracker.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the Engagement Tracker application.
/// Configures entity relationships, indexes, and constraints using the Fluent API.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUser(modelBuilder);
        ConfigureClient(modelBuilder);
        ConfigureEngagement(modelBuilder);
        ConfigureTimeEntry(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
        });
    }

    private static void ConfigureClient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(c => c.Name)
                .IsUnique();

            entity.Property(c => c.Industry)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.ContactEmail)
                .IsRequired()
                .HasMaxLength(255);
        });
    }

    private static void ConfigureEngagement(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Engagement>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.BudgetHours)
                .HasPrecision(10, 2);

            entity.Property(e => e.HourlyRate)
                .HasPrecision(10, 2);

            entity.HasOne(e => e.Client)
                .WithMany(c => c.Engagements)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Manager)
                .WithMany(u => u.ManagedEngagements)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ManagerId);
            entity.HasIndex(e => e.ClientId);
        });
    }

    private static void ConfigureTimeEntry(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Hours)
                .HasPrecision(5, 2);

            entity.Property(t => t.Description)
                .HasMaxLength(500);

            entity.HasOne(t => t.User)
                .WithMany(u => u.TimeEntries)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Engagement)
                .WithMany(e => e.TimeEntries)
                .HasForeignKey(t => t.EngagementId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(t => t.UserId);
            entity.HasIndex(t => t.EngagementId);
            entity.HasIndex(t => t.Date);
        });
    }
}
