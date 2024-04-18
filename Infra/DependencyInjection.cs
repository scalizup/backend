using Application.Common.Interfaces;
using Application.Repositories;
using Infra.Configuration;
using Infra.Configuration.Options;
using Infra.Repositories;
using Infra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra;

public class TenantAccessRequirement(IEnumerable<int> availableTenants) : IAuthorizationRequirement
{
    public IEnumerable<int> AvailableTenants { get; set; } = availableTenants;
}

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services
            .AddScoped<ITenantRepository, TenantRepository>()
            .AddScoped<ITagGroupRepository, TagGroupRepository>()
            .AddScoped<ITagRepository, TagRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IRoleRepository, RoleRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddOptions<JwtConfiguration>()
            .Bind(configuration.GetSection("Jwt"));

        services
            .ConfigureOptions<JwtOptions>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer()
            .Services
            .AddAuthorization();

        services
            .AddScoped<ITokenService, TokenService>();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}