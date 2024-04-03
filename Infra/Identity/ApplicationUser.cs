using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infra.Identity;

public class ApplicationUser : IdentityUser
{
    public IList<Tenant> AvailableTenants { get; set; } = [];
}