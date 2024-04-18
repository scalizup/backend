using Domain.Entities.Common;

namespace Domain.Entities;

public class Tenant(
    string name) : BaseEntity
{
    public string Name { get; set; } = name;

    public bool IsActive { get; set; }

    public List<TagGroup> TagGroups { get; set; } = default!;
    
    public List<User> Users { get; set; } = default!;
    
    public List<RefreshToken> RefreshTokens { get; set; } = default!;
}