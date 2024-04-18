using Application.Common.Interfaces;
using Domain.Constants;

namespace Application.Common.Security;

[Authorize(Policy = Policies.NeedsTenantAccess)]
public record BasePermissionRequest : IBaseRequest
{
    public IUserAccessor UserAccessor { get; set; } = default!;
    
    public int TenantId { get; set; }
}