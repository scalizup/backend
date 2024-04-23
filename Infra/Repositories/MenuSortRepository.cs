using Application.Repositories;
using Domain.Entities.Menu;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class MenuSortRepository(AppDbContext context) : IMenuSortRepository
{
    public async Task<int> UpsertAsync(MenuSort menuSort, CancellationToken cancellationToken)
    {
        var entity = await context.MenuSorts
            .SingleOrDefaultAsync(x => x.TenantId == menuSort.TenantId, cancellationToken);

        if (entity is null)
        {
            await context.MenuSorts.AddAsync(menuSort, cancellationToken);
        }
        else
        {
            entity.TagGroupId = menuSort.TagGroupId;
            entity.ProductsTagOrders = menuSort.ProductsTagOrders;

            context.MenuSorts.Update(entity);
        }

        await context.SaveChangesAsync(cancellationToken);

        return menuSort.Id;
    }

    public Task<MenuSort?> GetAsync(int tenantId, CancellationToken cancellationToken)
    {
        return context.MenuSorts
            .SingleOrDefaultAsync(ms => ms.TenantId == tenantId, cancellationToken);
    }
}