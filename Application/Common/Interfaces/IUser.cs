namespace Application.Common.Interfaces;

public interface IUser
{
    int? Id { get; }

    public string RequestIp { get; set; }
}