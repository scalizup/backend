using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IUserAccessor
{
    public string RequestIp { get; set; }

    public User User { get; set; }
    
    public RefreshToken RefreshToken { get; set; }
}