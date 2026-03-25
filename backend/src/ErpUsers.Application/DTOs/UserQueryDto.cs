namespace ErpUsers.Application.DTOs;

/// <summary>
/// Encapsulates all query parameters for user listing — avoids a long parameter list
/// and makes the service interface stable as new filters are added (OCP).
/// </summary>
public record UserQueryDto(
    string? Search = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10
);
