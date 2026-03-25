using ErpUsers.Domain.Interfaces;
using ErpUsers.Infrastructure.Data;
using ErpUsers.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ErpUsers.Infrastructure.Extensions;

/// <summary>
/// Registers all Infrastructure services.
/// IConfiguration is used here only to configure EF Core's UseNpgsql.
/// UserRepository no longer needs it — it reuses the DbContext connection.
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.EnableRetryOnFailure(3)));

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
