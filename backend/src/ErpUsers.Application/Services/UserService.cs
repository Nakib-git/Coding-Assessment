using ErpUsers.Application.DTOs;
using ErpUsers.Application.Interfaces;
using ErpUsers.Domain.Entities;
using ErpUsers.Domain.Interfaces;

namespace ErpUsers.Application.Services;

/// <summary>
/// Orchestrates business logic: validates input ranges, delegates persistence
/// to the repository, and projects domain entities to DTOs.
/// Single-Responsibility: only orchestrates — no SQL or HTTP concerns here.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository) => _repository = repository;

    public async Task<PagedResult<UserDto>> GetUsersAsync(
        UserQueryDto query, CancellationToken ct = default)
    {
        var page     = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var (users, total) = await _repository.GetPagedAsync(
            query.Search, query.IsActive, page, pageSize, ct);

        return new PagedResult<UserDto>(users.Select(ToDto), total, page, pageSize);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        var user    = new User(dto.Name, dto.Email, dto.Role, dto.IsActive);
        var created = await _repository.CreateAsync(user, ct);
        return ToDto(created);
    }

    public async Task<UserDto?> UpdateUserAsync(
        Guid id, UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return null;

        user.Update(dto.Name, dto.Email, dto.Role, dto.IsActive);
        var updated = await _repository.UpdateAsync(user, ct);
        return updated is null ? null : ToDto(updated);
    }

    public Task<bool> DeleteUserAsync(Guid id, CancellationToken ct = default)
        => _repository.DeleteAsync(id, ct);

    // ── private mapper (kept close to usage; no AutoMapper dependency needed) ──
    private static UserDto ToDto(User u) =>
        new(u.Id, u.Name, u.Email, u.Role, u.IsActive, u.CreatedAt, u.UpdatedAt);
}
