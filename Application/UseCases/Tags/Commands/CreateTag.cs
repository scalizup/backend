using Application.Repositories;
using Domain.Entities;

namespace Application.UseCases.Tags.Commands;

public static class CreateTag
{
    public record Command(
        int TagGroupId,
        string Name) : BasePermissionRequest, IRequest<int>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(ITagRepository tagRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var tagGroup = new Tag(
                request.TenantId,
                request.TagGroupId,
                request.Name);

            var newTag = await tagRepository.CreateTag(tagGroup, cancellationToken);

            return newTag;
        }
    }
}