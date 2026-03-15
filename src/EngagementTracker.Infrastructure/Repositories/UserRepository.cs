using EngagementTracker.Core.Interfaces;
using EngagementTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EngagementTracker.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the user repository.
/// Maps Infrastructure entities to Core's UserRecord.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<UserRecord?> GetByEmailAsync(string email)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        return user == null ? null : ToRecord(user);
    }

    /// <inheritdoc />
    public async Task<UserRecord?> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : ToRecord(user);
    }

    private static UserRecord ToRecord(Entities.User user) =>
        new(user.Id, user.Email, user.PasswordHash, user.Role, user.FirstName, user.LastName);
}
