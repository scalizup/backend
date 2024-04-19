using Domain.Entities.Menu;

namespace Application.Repositories;

public interface IPropertyOrderRepository
{
    Task<int> UpsertPropertyOrderAsync(PropertyOrder propertyOrder, CancellationToken cancellationToken);
    
    Task<PropertyOrder?> GetAsync(int tenantId, CancellationToken cancellationToken);
}