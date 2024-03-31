namespace Domain.Entities;

public class User(
    string username,
    string password)
{
    public Guid Id { get; set; }
    
    public string Username { get; set; } = username;
    
    public string Password { get; set; } = password;

    public bool IsActive { get; set; }
    
    public IEnumerable<Tenant> Tenants { get; set; } = [];
}