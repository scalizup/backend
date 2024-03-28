using Domain.Entities.Common;

namespace Domain.Entities;

public class TagGroup(
    int tenantId,
    string name) : TenantAware(tenantId)
{
    public string Name { get; set; } = name;
    
    public IEnumerable<Tag> Tags { get; set; } = default!;
}