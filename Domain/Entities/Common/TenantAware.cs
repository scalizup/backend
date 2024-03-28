namespace Domain.Entities.Common;

public class TenantAware : BaseEntity
{
    protected TenantAware(int tenantId)
    {
        TenantId = tenantId;
    }
    
    public int TenantId { get; init; }
}