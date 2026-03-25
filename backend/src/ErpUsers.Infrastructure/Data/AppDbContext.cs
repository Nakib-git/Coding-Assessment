using ErpUsers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpUsers.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            entity.Property(u => u.Role).HasColumnName("role").HasMaxLength(50).IsRequired();
            entity.Property(u => u.IsActive).HasColumnName("is_active");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            entity.Property(u => u.UpdatedAt).HasColumnName("updated_at");

            // Unique constraint enforced at DB level
            entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("ix_users_email_unique");

            // Supports fast filter-by-status
            entity.HasIndex(u => u.IsActive).HasDatabaseName("ix_users_is_active");
        });

        base.OnModelCreating(modelBuilder);
    }
}
