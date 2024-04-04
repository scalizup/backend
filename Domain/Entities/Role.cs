using Domain.Entities.Common;

namespace Domain.Entities;

public class Role(
    string name) : BaseEntity
{
    public string Name { get; set; } = name;

    public List<User> Users { get; set; } = [];
}