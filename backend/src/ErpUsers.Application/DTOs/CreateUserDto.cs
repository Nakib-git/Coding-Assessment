using System.ComponentModel.DataAnnotations;

namespace ErpUsers.Application.DTOs;

public record CreateUserDto(
    [Required, MaxLength(100)] string Name,
    [Required, EmailAddress, MaxLength(200)] string Email,
    [Required, MaxLength(50)] string Role,
    bool IsActive = true
);
