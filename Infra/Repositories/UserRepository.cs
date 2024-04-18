using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<int> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.AvailableTenants)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await context.Users
            .Include(u => u.AvailableTenants)
            .Include(u => u.Roles)
            .ToListAsync();
    }

    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        var existingUser = await context.Users
            .FindAsync(user.Id, cancellationToken);

        if (existingUser is null)
        {
            return false;
        }

        context.Entry(existingUser).CurrentValues.SetValues(user);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> IsInTenant(int userId, int tenantId)
    {
        return await context.Users
            .Include(u => u.AvailableTenants)
            .AnyAsync(u => u.Id == userId && u.AvailableTenants.Any(t => t.Id == tenantId));
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await context.Users
            .Include(u => u.AvailableTenants)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
}