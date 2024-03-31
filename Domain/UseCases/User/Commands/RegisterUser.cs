namespace Domain.UseCases.User.Commands;

public static class RegisterUser
{
    public record Command(
        string Username,
        string Password) : IRequest<Guid>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IUserRepository userRepository)
        {
            RuleFor(c => c.Username)
                .NotEmpty()
                .MinimumLength(2)
                .MustAsync(async (username, cancellationToken) =>
                {
                    var user = await userRepository.GetByUsernameAsync(username, cancellationToken);

                    return user is not null;
                })
                .WithMessage("Username already exists.");

            RuleFor(c => c.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }

    public class Handler(IUserRepository userRepository) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = new Entities.User(request.Username, request.Password);

            var userId = await userRepository.CreateUserAsync(user, cancellationToken);

            return userId;
        }
    }
}