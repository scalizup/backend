using Domain.Services;

namespace Domain.UseCases.User.Commands;

public static class LoginUser
{
    public record Command(
        string Username,
        string Password) : IRequest<Token>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IUserRepository userRepository)
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(2)
                .MustAsync(async (username, cancellationToken) =>
                {
                    var user = await userRepository.GetByUsernameAsync(username, cancellationToken);
                    
                    if (user is null || !user.IsActive)
                    {
                        return false;
                    }
                    
                    if (!user.Password.Equals(username))
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage("User not found or is inactive.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }

    public class Handler(
        ITokenService tokenService,
        IUserRepository userRepository) : IRequestHandler<Command, Token>
    {
        public async Task<Token> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            var token = await tokenService.GenerateTokenAsync(existingUser!, cancellationToken);

            return token;
        }
    }
}