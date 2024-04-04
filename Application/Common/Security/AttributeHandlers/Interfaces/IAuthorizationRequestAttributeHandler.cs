using Application.Common.Interfaces;
using MediatR;

namespace Application.Common.Security.AttributeHandlers.Interfaces;

public interface IAuthorizationRequestAttributeHandler
{
    Task<bool> Handle(IBaseRequest request, IUser user);
}