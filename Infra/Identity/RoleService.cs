using Application.Common.Interfaces;
using Application.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Infra.Identity;

public class RoleService(
    UserManager<ApplicationUser> userManager,
    ITenantRepository tenantRepository,
    RoleManager<IdentityRole> roleManager) : IRoleService
{
    public async Task<bool> AddUserToRole(string userId, string roleName)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await userManager.AddToRoleAsync(user, roleName);
        
        return result.Succeeded;
    }

    public async Task<bool> AddUserToTenant(string userId, int tenantId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        var tenant = await tenantRepository.GetTenantById(tenantId, default);
        if (tenant == null)
        {
            return false;
        }
        
        user.AvailableTenants.Add(tenant);
        
        var result = await userManager.UpdateAsync(user);
        
        return result.Succeeded;
    }

    public async Task<bool> RemoveUserFromRole(string userId, string roleName)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await userManager.RemoveFromRoleAsync(user, roleName);
        
        return result.Succeeded;
    }

    public async Task<bool> CreateRole(string roleName)
    {
        var role = new IdentityRole(roleName);
        var result = await roleManager.CreateAsync(role);

        return result.Succeeded;
    }

    public async Task<bool> DeleteRole(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return false;
        }

        var result = await roleManager.DeleteAsync(role);

        return result.Succeeded;
    }
}