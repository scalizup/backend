using Application.Common.Interfaces;
using Domain.Constants;
using MediatR;
namespace Application.Common.Security;

[Authorize(Policy = Policies.NeedsTenantAccess)]
public record BasePermissionRequest : IBaseRequest
{
    public IUser User { get; set; } = default!;
    
    public int TenantId { get; set; }
}