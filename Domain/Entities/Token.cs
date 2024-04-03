namespace Domain.Entities;

public record Token(
    string Value,
    string RefreshValue)
{
    public Guid UserId { get; set; }
}