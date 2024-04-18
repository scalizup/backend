using Application.Common.Exceptions;
using Application.Repositories;

namespace Application.UseCases.Tags.Commands;

public static class UpdateTag
{
    public record Command(
        int Id,
        string? Name = null) : BasePermissionRequest, IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0);

            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(2)
                .When(t => t.Name is not null);
        }
    }

    public class Handler(ITagRepository tagRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var tagGroup = await tagRepository.GetTagById(request.Id, cancellationToken);
            if (tagGroup is null)
            {
                throw new NotFoundException($"Tag with id {request.Id} was not found.");
            }

            var wasUpdated = await tagRepository.UpdateTag(
                request.Id,
                request.Name,
                cancellationToken);

            return wasUpdated;
        }
    }
}