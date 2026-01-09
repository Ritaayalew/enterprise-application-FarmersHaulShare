using IdentityAndAccessManagement.Application.Abstractions;
using IdentityAndAccessManagement.Domain.Entities;
using IdentityAndAccessManagement.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace IdentityAndAccessManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _db;

    public UserRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByKeycloakSubjectIdAsync(string subjectId, CancellationToken ct = default)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.KeycloakSubjectId == subjectId, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();

        return await _db.Users
            .FirstOrDefaultAsync(u => u.Email.Value == normalized, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        return Task.CompletedTask;
    }

}
