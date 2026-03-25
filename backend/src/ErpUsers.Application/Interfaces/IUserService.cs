using ErpUsers.Application.DTOs;

namespace ErpUsers.Application.Interfaces;

/// <summary>
/// Defines all CRUD operations plus paginated listing.
/// The controller depends only on this interface (ISP + DIP).
/// </summary>
public interface IUserService
{
    Task<PagedResult<UserDto>> GetUsersAsync(UserQueryDto query, CancellationToken ct = default);
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken ct = default);
}
