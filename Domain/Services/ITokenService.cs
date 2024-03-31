namespace Domain.Services;

public record Token(
    string Value,
    string RefreshValue);

public interface ITokenService
{
    Task<Token> GenerateTokenAsync(User user, CancellationToken cancellationToken);
}