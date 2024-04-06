using System.Reflection;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Repositories;
using MediatR;

namespace Application.Common.Security.AttributeHandlers.Interfaces;

public interface IAuthorizationRequestAttributeHandler
{
    Task<bool> Handle(IBaseRequest request, IUser user);
}

public static class AuthorizationRequestAttributeHandlerReflector
{
    public static async Task HandlePolicy(string policy, IBaseRequest request, IUserRepository userRepository, IUser user)
    {
        var handlerType = GetHandlerType(policy);

        if (handlerType is null)
        {
            throw new InvalidOperationException($"Handler for policy {policy} not found.");
        }

        await InvokeHandler(userRepository, user, handlerType, request);
    }

    private static async Task InvokeHandler(
        IUserRepository userRepository,
        IUser user,
        Type handlerType, 
        IBaseRequest request)
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
