namespace ErpUsers.Application.DTOs;

/// <summary>Read-only projection — never exposes the domain entity directly.</summary>
public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
