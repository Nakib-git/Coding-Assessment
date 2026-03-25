namespace ErpUsers.Domain.Entities;

/// <summary>
/// Core ERP User domain entity — encapsulates all business rules around mutation.
/// </summary>
public sealed class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Required by EF Core — not for external use
    private User() { }

    public User(string name, string email, string role, bool isActive = true)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Role = role;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string email, string role, bool isActive)
    {
        Name = name;
        Email = email;
        Role = role;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
