using Domain.Entities.Common;

namespace Domain.Entities;

public class TagGroup(
    string name) : TenantAware
{
    public string Name { get; set; } = name;
    
    public IEnumerable<Tag> Tags { get; set; } = default!;
}