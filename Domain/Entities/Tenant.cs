using Domain.Entities.Common;

namespace Domain.Entities;

public class Tenant(
    string name) : BaseEntity
{
    public string Name { get; set; } = name;

    public bool IsActive { get; set; }

    public IEnumerable<TagGroup> TagGroups { get; set; } = default!;
}