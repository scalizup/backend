using Application.Repositories;
using Domain.Entities.Menu;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class PropertyOrderRepository(AppDbContext context) : IPropertyOrderRepository
{
    public async Task<int> UpsertPropertyOrderAsync(PropertyOrder propertyOrder, CancellationToken cancellationToken)
    {
        var existingPropertyOrder = await context.PropertyOrders
            .FirstOrDefaultAsync(po => po.TenantId == propertyOrder.TenantId, cancellationToken);

        if (existingPropertyOrder != null)
        {
            existingPropertyOrder.TagGroupId = propertyOrder.TagGroupId;
            existingPropertyOrder.OrderOfIds = propertyOrder.OrderOfIds;
            context.PropertyOrders.Update(existingPropertyOrder);
        }
        else
        {
            await context.PropertyOrders.AddAsync(propertyOrder, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return propertyOrder.Id;
    }

    public async Task<PropertyOrder?> GetAsync(int tenantId, CancellationToken cancellationToken)
    {
        return await context.PropertyOrders
            .FirstOrDefaultAsync(po => po.TenantId == tenantId, cancellationToken);
    }
}