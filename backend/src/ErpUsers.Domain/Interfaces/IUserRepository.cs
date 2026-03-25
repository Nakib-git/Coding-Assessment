using ErpUsers.Domain.Entities;

namespace ErpUsers.Domain.Interfaces;

/// <summary>
/// Repository contract — lives in Domain so the Application layer can depend on it
/// without referencing Infrastructure directly (Dependency Inversion Principle).
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Returns a page of users matching the filters plus the total row count.
    /// The count MUST be computed via ADO.NET (see Infrastructure implementation).
    /// </summary>
    Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(
        string? search,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task<User?> UpdateAsync(User user, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
