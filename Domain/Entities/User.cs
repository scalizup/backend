using Domain.Entities.Common;

namespace Domain.Entities;

public class User(
    string username,
    string password) : BaseEntity
{
    public string Username { get; set; } = username;

    public string Password { get; set; } = password;

    public List<Role> Roles { get; set; } = new();

    public List<Tenant> AvailableTenants { get; set; } = [];
}