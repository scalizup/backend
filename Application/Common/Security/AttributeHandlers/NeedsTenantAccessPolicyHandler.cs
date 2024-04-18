using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Security.AttributeHandlers.Interfaces;
using Application.Repositories;
using Domain.Constants;

namespace Application.Common.Security.AttributeHandlers;

[AuthorizationRequest(Policies.NeedsTenantAccess)]
public class NeedsTenantAccessPolicyHandler(IUserRepository userRepository)
    : IAuthorizationRequestAttributeHandler
{
    public async Task<bool> Handle(
        IBaseRequest request,
        IUserAccessor userAccessor)
    {
        var tenantId = (request as BasePermissionRequest)!.TenantId;
        if (tenantId < 1)
        {
            throw new ForbiddenAccessException("The user does not have access to any tenant.");
        }

        return await userRepository.IsInTenant(userAccessor.User.Id, tenantId);
    }
}