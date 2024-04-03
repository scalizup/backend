using System.Security.Claims;

namespace Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }

    ClaimsPrincipal? ClaimsPrincipal { get; }
    
    int TenantId { get; }
}