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
    IUserRepository userRepository)
    : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly string[] _priorityRolesAndPolicies =
    [
        UserRoles.Admin
    ];

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();
        
        if (authorizeAttributes.Count != 0)
        {
            if (user.Id is null)
            {
                throw new UnauthorizedAccessException();
            }

            var itIsPriorityRole = false;
            if (request is BasePermissionRequest basePermissionRequest)
            {
                var existingUser = await userRepository.GetUserByIdAsync(user.Id.Value, cancellationToken);
                basePermissionRequest.User = user;

                var tenantId = request.GetType()
                    .GetProperty(nameof(basePermissionRequest.TenantId))
                    ?.GetValue(request) as int? ?? 0;

                itIsPriorityRole = existingUser!.Roles.Select(r => r.Name)
                    .Any(r => _priorityRolesAndPolicies.Contains(r));

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
                    : existingUser.AvailableTenants.FirstOrDefault()?.Id ?? 0;
            }

            var authorizeAttributesWithPolicies = GetPoliciesFromAttributes(authorizeAttributes);

            var authorized = false;
            if (authorizeAttributesWithPolicies.Roles.Length == 0)
            {
                authorized = true;
            }
            else
            {
                foreach (var role in authorizeAttributesWithPolicies.Roles)
                {
                    var isInRole = await roleRepository.IsInRoleAsync(user.Id.Value, role);
                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }
            }

            if (!authorized)
            {
                throw new ForbiddenAccessException();
            }

            if (authorizeAttributesWithPolicies.Policies.Length != 0 && !itIsPriorityRole)
            {
                foreach (var policy in authorizeAttributesWithPolicies.Policies)
                {
                    await HandlePolicy(policy, (IBaseRequest)request);
                }
            }
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

    private async Task HandlePolicy(string policy, IBaseRequest request)
    {
        var handlerType = GetHandlerType(policy);

        if (handlerType is null)
        {
            throw new InvalidOperationException($"Handler for policy {policy} not found.");
        }

        await InvokeHandler(handlerType, request);
    }

    private async Task InvokeHandler(Type handlerType, IBaseRequest request)
    {
        var handler = Activator.CreateInstance(handlerType, userRepository);
        var handleMethod = handlerType.GetMethod("Handle");
        var result = await (Task<bool>)handleMethod?.Invoke(handler, [request, user])!;

        if (!result)
        {
            throw new ForbiddenAccessException();
        }
    }

    private static Type? GetHandlerType(string policy)
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .FirstOrDefault(t =>
                t.GetInterfaces().Any(i => i.IsAssignableTo(typeof(IAuthorizationRequestAttributeHandler)))
                && t.GetCustomAttributes<AuthorizationRequestAttribute>().Any(a => a.Name == policy));
    }
}