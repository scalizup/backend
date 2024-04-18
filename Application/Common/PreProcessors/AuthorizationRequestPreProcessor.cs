using System.Reflection;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Security.AttributeHandlers.Interfaces;
using Application.Repositories;
using Domain.Constants;
using Domain.Entities;
using MediatR.Pipeline;

namespace Application.Common.PreProcessors;

public class AuthorizationRequestPreProcessor<TRequest>(
    IUserAccessor userAccessor,
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
            var refreshToken = await IsValidRequest(cancellationToken);

            var itIsPriorityRole = false;
            if (request is BasePermissionRequest basePermissionRequest)
            {
                var existingUser = await userRepository.GetUserByIdAsync(userAccessor.User.Id, cancellationToken);
                itIsPriorityRole = existingUser!.Roles.Select(r => r.Name)
                    .Any(r => _priorityRolesAndPolicies.Contains(r));

                basePermissionRequest.UserAccessor = userAccessor;
                basePermissionRequest.UserAccessor.RefreshToken = refreshToken;
                basePermissionRequest.UserAccessor.User = existingUser;
                basePermissionRequest.TenantId = existingUser.AvailableTenants.First().Id;

                var tenantId = request.GetType()
                    .GetProperty(nameof(basePermissionRequest.TenantId))
                    ?.GetValue(request) as int? ?? 0;

                if (!itIsPriorityRole)
                {
                    var availableTenants = existingUser.AvailableTenants.Select(t => t.Id).ToList();
                    if (availableTenants.Count == 0)
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
                    var isInRole = await roleRepository.IsInRoleAsync(userAccessor.User.Id, role);
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
                        userRepository, userAccessor);
                }
            }
        }
    }

    private async Task<RefreshToken> IsValidRequest(CancellationToken cancellationToken)
    {
        if (userAccessor.User is null || string.IsNullOrEmpty(userAccessor.RefreshToken.Token))
        {
            throw new UnauthorizedAccessException();
        }

        var refreshToken = await refreshTokenRepository
            .GetRefreshToken(userAccessor.RefreshToken.Token, cancellationToken);

        if (refreshToken is null || refreshToken.IsBlacklisted || refreshToken.IsActive is false)
        {
            throw new UnauthorizedAccessException("Refresh token is blacklisted. Please log in again.");
        }

        return refreshToken;
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