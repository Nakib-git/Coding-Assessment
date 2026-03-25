using ErpUsers.Application.DTOs;
using ErpUsers.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ErpUsers.API.Controllers;

/// <summary>
/// Thin controller — delegates ALL logic to IUserService (SRP + DIP).
/// One action per HTTP verb, no business rules here.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    // GET /api/users?search=&isActive=true&page=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct     = default)
    {
        var query  = new UserQueryDto(search, isActive, page, pageSize);
        var result = await _service.GetUsersAsync(query, ct);
        return Ok(result);
    }

    // GET /api/users/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct = default)
    {
        var user = await _service.GetUserByIdAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    // POST /api/users
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _service.CreateUserAsync(dto, ct);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // PUT /api/users/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        Guid id, [FromBody] UpdateUserDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _service.UpdateUserAsync(id, dto, ct);
        return user is null ? NotFound() : Ok(user);
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct = default)
    {
        var deleted = await _service.DeleteUserAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
