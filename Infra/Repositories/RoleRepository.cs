using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class RoleRepository(AppDbContext context) : IRoleRepository
{
    public async Task<int> CreateRoleAsync(Role role, CancellationToken cancellationToken)
    {
        context.Roles.Add(role);

        await context.SaveChangesAsync(cancellationToken);

        return role.Id;
    }

    public async Task<IEnumerable<Role>> GetAllRoles(CancellationToken cancellationToken)
    {
        return await context.Roles.ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        return await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
    }

    public async Task<bool> IsInRoleAsync(int userId, string role)
    {
        var user = await context.Users
            .Include(r => r.Roles)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .Where(r => r.Name == role)
            .FirstOrDefaultAsync();

        return user is not null;
    }

    public async Task<bool> AddRoleToUserAsync(int userId, int roleId, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(r => r.Roles)
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return false;
        }

        var role = await context.Roles.FindAsync(roleId);

        if (role is null || user.Roles.Any(r => r.Name == role.Name))
        {
            return false;
        }
        
        user.Roles.Add(role);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(r => r.Roles)
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return false;
        }

        var role = await context.Roles.FindAsync(roleId);

        if (role is null)
        {
            return false;
        }

        user.Roles.Remove(role);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}