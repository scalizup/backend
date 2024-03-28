using Domain.Entities.Common;

namespace Domain.Entities;

public class Tag(
    int tenantId,
    string name) : TenantAware(tenantId)
{
    public string Name { get; set; } = name;
    
    public IEnumerable<TagGroup> TagGroups { get; set; } = default!;
}