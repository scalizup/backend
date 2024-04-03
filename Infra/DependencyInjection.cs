using Application.Common.Interfaces;
using Application.Repositories;
using Domain.Constants;
using Infra.Identity;
using Infra.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
            .AddScoped<ITagRepository, TagRepository>();

        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));
            }
        );

        return services;
    }
}