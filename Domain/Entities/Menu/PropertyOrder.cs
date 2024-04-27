using Domain.Entities.Common;

namespace Domain.Entities.Menu;

public class MenuSort(
    int tenantId) : TenantAware(tenantId)
{
    public int TagGroupId { get; set; }

    public ICollection<ProductsTagOrder> ProductsTagOrders { get; set; } = [];
}

public class ProductsTagOrder : BaseEntity
{
    public int TagId { get; set; }
    
    public ICollection<int> ProductsIds { get; set; } = [];
}