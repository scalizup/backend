namespace Application.Common.Interfaces;

public interface IRoleService
{
    Task<bool> AddUserToRole(string userId, string roleName);
    
    Task<bool> AddUserToTenant(string userId, int tenantId);

    Task<bool> RemoveUserFromRole(string userId, string roleName);

    Task<bool> CreateRole(string roleName);

    Task<bool> DeleteRole(string roleName);
}