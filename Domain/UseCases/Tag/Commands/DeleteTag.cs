namespace Domain.UseCases.Tag.Commands;

public static class DeleteTag
{
    public record Command(
        int Id) : IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }

    public class Handler(ITagRepository tagRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingTag = await tagRepository.GetTagById(request.Id, cancellationToken);
            if (existingTag is null)
            {
                throw new NotFoundException($"Tag with id {request.Id} was not found.");
            }

            var wasDeleted = await tagRepository.DeleteTag(request.Id, cancellationToken);

            return wasDeleted;
        }
    }
}