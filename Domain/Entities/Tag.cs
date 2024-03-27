using Domain.Entities.Common;

namespace Domain.Entities;

public class Tag(
    string name) : TenantAware
{
    public string Name { get; set; } = name;
    
    public IEnumerable<TagGroup> TagGroups { get; set; } = default!;
}