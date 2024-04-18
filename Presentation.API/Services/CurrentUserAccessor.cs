using System.Security.Claims;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Presentation.API.Services;

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : IUserAccessor
{
    public ClaimsPrincipal? ClaimsPrincipal => httpContextAccessor.HttpContext?.User;

    public string RequestIp { get; set; } = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";

    public User User { get; set; } = new()
    {
        Id = int.Parse(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0")
    };

    public RefreshToken RefreshToken { get; set; } = new()
    {
        Token = httpContextAccessor.HttpContext?.Request.Headers["refreshToken"].FirstOrDefault() ?? ""
    };
}