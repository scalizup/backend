using Domain.Entities.Common;

namespace Domain.Entities.Menu;

public class PropertyOrder(
    int tenantId,
    int tagGroupId,
    List<int> orderOfIds) : TenantAware(tenantId)
{
    public int TagGroupId { get; set; } = tagGroupId;

    public List<int> OrderOfIds { get; set; } = orderOfIds;
}