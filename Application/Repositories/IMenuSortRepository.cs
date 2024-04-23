using Domain.Entities.Menu;

namespace Application.Repositories;

public interface IMenuSortRepository
{
    Task<int> UpsertAsync(MenuSort menuSort, CancellationToken cancellationToken);
    
    Task<MenuSort?> GetAsync(int tenantId, CancellationToken cancellationToken);
}