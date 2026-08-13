using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Infrastructure.Auth;
using ResalePlatform.Infrastructure.Identity;
using ResalePlatform.Infrastructure.Persistence;

namespace ResalePlatform.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Регистрирует инфраструктурные сервисы (EF Core / PostgreSQL / Identity / JWT).</summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                       // Повтор при временной недоступности БД — важно при старте в Docker.
                       npgsql.EnableRetryOnFailure(maxRetryCount: 10,
                           maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null))
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
