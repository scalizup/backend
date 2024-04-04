using Domain.Entities;

namespace Application.Repositories;

public interface IUserRepository
{
    Task<int> CreateUserAsync(User user, CancellationToken cancellationToken);

    Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken);

    Task<IEnumerable<User>> GetAllUsers();

    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken);
    
    Task<bool> IsInTenant(int userId, int tenantId);

    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
}