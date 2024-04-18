using Domain.Entities.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public User()
    {
    }

    public User(string? username, string? password)
    {
        Username = username;
        Password = password;
    }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public List<Role> Roles { get; set; } = [];

    public List<Tenant> AvailableTenants { get; set; } = [];
}