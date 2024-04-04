using Domain.Entities;

namespace Application.Repositories;

public interface IRoleRepository
{
    Task<int> CreateRoleAsync(Role role, CancellationToken cancellationToken);

    Task<IEnumerable<Role>> GetAllRoles(CancellationToken cancellationToken);

    Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);

    Task<bool> IsInRoleAsync(int userId, string role);

    Task<bool> AddRoleToUserAsync(int userId, int roleId, CancellationToken cancellationToken);

    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken);
    
}