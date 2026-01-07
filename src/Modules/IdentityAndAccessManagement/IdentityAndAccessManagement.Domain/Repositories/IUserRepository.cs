using IdentityAndAccessManagement.Domain.Entities;

namespace IdentityAndAccessManagement.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByKeycloakSubjectIdAsync(string subjectId, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}