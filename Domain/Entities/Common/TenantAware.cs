namespace Domain.Entities.Common;

public class TenantAware : BaseEntity
{
    public int TenantId { get; init; }
}