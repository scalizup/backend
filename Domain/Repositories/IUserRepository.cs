namespace Domain.Repositories;

public interface IUserRepository
{
    Task<Guid> CreateUserAsync(User user, CancellationToken cancellationToken);
    
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
}