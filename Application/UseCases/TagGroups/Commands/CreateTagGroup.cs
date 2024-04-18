using Application.Repositories;
using Domain.Entities;

namespace Application.UseCases.TagGroups.Commands;

public static class CreateTagGroup
{
    public record Command(
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

    public class Handler(ITagGroupRepository tagGroupRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var tagGroup = new TagGroup(
                request.TenantId,
                request.Name);

            var newTagGroup = await tagGroupRepository.CreateTagGroup(tagGroup, cancellationToken);

            return newTagGroup;
        }
    }
}