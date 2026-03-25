using System.Data;
using System.Text;
using ErpUsers.Domain.Entities;
using ErpUsers.Domain.Interfaces;
using ErpUsers.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace ErpUsers.Infrastructure.Repositories;

/// <summary>
/// Implements IUserRepository with a hybrid strategy:
///   • EF Core  — main data query (typed, composable, migration-friendly)
///   • ADO.NET  — optimised COUNT query (avoids EF overhead for a scalar)
///
/// ADO.NET connection strategy:
///   We reuse the DbConnection that EF Core already manages via
///   _context.Database.GetDbConnection().  This means:
///     - No second connection string injection (no IConfiguration needed)
///     - No extra pool slot consumed
///     - Automatically participates in any active EF transaction
///     - We open/close the connection ourselves only when EF Core has not
///       already opened it (tracked via the ConnectionState check).
///
/// SOLID notes:
///   SRP  — only handles data access, no business logic
///   OCP  — new filters extend GetCountAsync + EF query without touching callers
///   LSP  — fully substitutes IUserRepository
///   DIP  — depends on abstractions (DbContext via DI, not IConfiguration)
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    // GET PAGED  (EF Core query + ADO.NET count)
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(
        string? search, bool? isActive, int page, int pageSize, CancellationToken ct = default)
    {
        // ADO.NET: optimised scalar COUNT 
        var totalCount = await GetCountAdoAsync(search, isActive, ct);

        // EF Core: typed, composable main query
        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            // ILIKE = native PostgreSQL case-insensitive LIKE (uses GIN/trigram index)
            query = query.Where(u =>
                EF.Functions.ILike(u.Name, pattern) ||
                EF.Functions.ILike(u.Email, pattern));
        }

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        var users = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (users, totalCount);
    }

    // PRIVATE: ADO.NET count — reuses EF Core connection, fully parameterised
    private async Task<int> GetCountAdoAsync(
        string? search, bool? isActive, CancellationToken ct)
    {
        var sql        = new StringBuilder("SELECT COUNT(*) FROM users WHERE 1=1");
        var parameters = new List<NpgsqlParameter>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Parameterised ILIKE — safe against SQL injection; uses trigram GIN index
            sql.Append(" AND (name ILIKE @search OR email ILIKE @search)");
            parameters.Add(new NpgsqlParameter("@search", NpgsqlDbType.Text)
            {
                Value = $"%{search}%"
            });
        }

        if (isActive.HasValue)
        {
            sql.Append(" AND is_active = @isActive");
            parameters.Add(new NpgsqlParameter("@isActive", NpgsqlDbType.Boolean)
            {
                Value = isActive.Value
            });
        }

        // Reuse EF Core's managed connection
        // GetDbConnection() returns the underlying Npgsql connection without
        // opening it.  We check ConnectionState so we leave it as we found it.
        var conn     = (NpgsqlConnection)_context.Database.GetDbConnection();
        var wasOpen  = conn.State == ConnectionState.Open;

        if (!wasOpen)
            await conn.OpenAsync(ct);

        try
        {
            await using var cmd = new NpgsqlCommand(sql.ToString(), conn);
            cmd.Parameters.AddRange(parameters.ToArray());

            var result = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(result);
        }
        finally
        {
            // Only close if WE opened it — don't interfere with EF Core's lifecycle
            if (!wasOpen)
                await conn.CloseAsync();
        }
    }

    // CRUD

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken ct = default)
    {
        // Attach detached entity and mark as modified
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, ct);
        if (user is null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _context.Users.AsNoTracking().AnyAsync(u => u.Id == id, ct);
}
