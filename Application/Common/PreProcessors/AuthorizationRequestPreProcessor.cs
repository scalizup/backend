using System.Reflection;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Security.AttributeHandlers.Interfaces;
using Application.Repositories;
using Domain.Constants;
using MediatR;
using MediatR.Pipeline;

namespace Application.Common.PreProcessors;

public class AuthorizationRequestPreProcessor<TRequest>(
    IUser user,
    IRoleRepository roleRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository)
    : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly string[] _priorityRolesAndPolicies = [UserRoles.Admin];

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (authorizeAttributes.Count != 0)
        {
            await IsValidRequest(cancellationToken);

            var itIsPriorityRole = false;
            if (request is BasePermissionRequest basePermissionRequest)
            {
                var existingUser = await userRepository.GetUserByIdAsync(user.Id!.Value, cancellationToken);
                itIsPriorityRole = existingUser!.Roles.Select(r => r.Name)
                    .Any(r => _priorityRolesAndPolicies.Contains(r));

                basePermissionRequest.User = user;

                var tenantId = request.GetType()
                    .GetProperty(nameof(basePermissionRequest.TenantId))
                    ?.GetValue(request) as int? ?? 0;

                if (!itIsPriorityRole)
                {
                    var availableTenants = existingUser.AvailableTenants.Select(t => t.Id).ToList();
                    if (availableTenants.Count != 0)
                    {
                        throw new ForbiddenAccessException("The user is not assigned to any tenant.");
                    }

                    var isUserInTenant = availableTenants.Contains(tenantId);
                    if (!isUserInTenant)
                    {
                        throw new ForbiddenAccessException("The user does not have access to the requested tenant.");
                    }
                }

                basePermissionRequest.TenantId = tenantId > 0
                    ? tenantId
                    : existingUser.AvailableTenants.First().Id;
            }

            var authorizeAttributesWithPolicies = GetPoliciesFromAttributes(authorizeAttributes);

            var authorized = false;

            if (authorizeAttributesWithPolicies.Roles.Length > 0 && !itIsPriorityRole)
            {
                foreach (var role in authorizeAttributesWithPolicies.Roles)
                {
                    var isInRole = await roleRepository.IsInRoleAsync(user.Id!.Value, role);
                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }
            }
            else
            {
                authorized = true;
            }

            if (!authorized)
            {
                throw new ForbiddenAccessException();
            }

            if (authorizeAttributesWithPolicies.Policies.Length != 0 && !itIsPriorityRole)
            {
                foreach (var policy in authorizeAttributesWithPolicies.Policies)
                {
                    await AuthorizationRequestAttributeHandlerReflector.HandlePolicy(
                        policy, 
                        (IBaseRequest)request,
                        userRepository, user);
                }
            }
        }
    }

    private async Task IsValidRequest(CancellationToken cancellationToken)
    {
        if (user.Id is null || string.IsNullOrEmpty(user.RefreshToken))
        {
            throw new UnauthorizedAccessException();
        }

        var refreshToken = await refreshTokenRepository
            .GetRefreshToken(user.RefreshToken, cancellationToken);
        
        if (refreshToken is null || refreshToken.IsBlacklisted || refreshToken.IsActive is false)
        {
            throw new UnauthorizedAccessException("Refresh token is blacklisted. Please log in again.");
        }
    }

    private (string[] Roles, string[] Policies) GetPoliciesFromAttributes(IList<AuthorizeAttribute> authorizeAttributes)
    {
        var policiesAttributes = authorizeAttributes
            .Select(a => a.Policy)
            .Where(p => !string.IsNullOrEmpty(p))
            .ToArray();

        var rolesAttributes = authorizeAttributes
            .SelectMany(a => a.Roles)
            .Where(p => !string.IsNullOrEmpty(p))
            .ToArray();

        return (rolesAttributes, policiesAttributes);
    }
}