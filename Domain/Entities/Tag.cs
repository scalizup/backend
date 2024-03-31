using Domain.Entities.Common;

namespace Domain.Entities;

public class Tag(
    int tenantId,
    int tagGroupId,
    string name) : TenantAware(tenantId)
{
    public string Name { get; set; } = name;

    public int TagGroupId { get; set; } = tagGroupId;

    public TagGroup TagGroup { get; set; } = default!;
}