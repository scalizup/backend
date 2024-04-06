using System.Security.Claims;
using Application.Common.Interfaces;

namespace Presentation.API.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public int? Id =>  ClaimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) is { } id
        ? int.Parse(id)
        : null;

    public ClaimsPrincipal? ClaimsPrincipal => httpContextAccessor.HttpContext?.User;

    public int TenantId { get; set; }

    public IEnumerable<string> Roles { get; set; } = [];

    public string RequestIp { get; set; } = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";

    public string RefreshToken { get; set; } =
        httpContextAccessor.HttpContext?.Request.Headers["refreshToken"].FirstOrDefault() ?? "";
}