using System.Reflection;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using MediatR.Pipeline;

namespace Application.Common.PreProcessors;

public class AuthorizationRequestPreProcessor<TRequest>(IUser user, IIdentityService identityService)
    : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (authorizeAttributes.Count != 0)
        {
            // Must be authenticated user
            if (user.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
                .ToList();

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await identityService.IsInRoleAsync(user.Id, role.Trim());
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
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = GetPoliciesFromAttributes(authorizeAttributes);
            if (authorizeAttributesWithPolicies.Length != 0)
            {
                foreach (var policy in authorizeAttributesWithPolicies)
                {
                    await HandlePolicy(policy, (IBaseRequest)request);
                }
            }
        }
    }

    private string[] GetPoliciesFromAttributes(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        return authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
            .Select(a => a.Policy)
            .ToArray();
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
        var handler = Activator.CreateInstance(handlerType, identityService);
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
                && t.Name.Contains(policy));
    }
}

public interface IAuthorizationRequestAttributeHandler
{
    Task<bool> Handle(IBaseRequest request, IUser user);
}

public class NeedsTenantAccessPolicyHandler(IIdentityService identityService)
    : IAuthorizationRequestAttributeHandler
{
    public async Task<bool> Handle(
        IBaseRequest request,
        IUser user)
    {
        var tenantId = (request as BasePermissionRequest)!.TenantId;
        if (tenantId < 1)
        {
            throw new ForbiddenAccessException();
        }

        return await identityService.IsInTenant(user.Id!, tenantId);
    }
}