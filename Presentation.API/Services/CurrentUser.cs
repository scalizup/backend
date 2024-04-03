using System.Security.Claims;
using Application.Common.Interfaces;

namespace Presentation.API.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public string? Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public ClaimsPrincipal? ClaimsPrincipal => httpContextAccessor.HttpContext?.User;
}