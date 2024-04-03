using Domain.Constants;
using MediatR;

namespace Application.Common.Security;

[Authorize(Policy = Policies.NeedsTenantAccess)]
public record BasePermissionRequest(
    int TenantId) : IBaseRequest;