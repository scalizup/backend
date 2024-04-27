using Application.Repositories;

namespace Application.UseCases.TagGroups.Queries;

public static class GetTagGroupById
{
    public record Query(
        int TagGroupId) : BasePermissionRequest, IRequest<TagGroupDto>;

    public class Validator : AbstractValidator<Query>
    {
        public Validator(ITagGroupRepository tagGroupRepository)
        {
            RuleFor(x => x.TagGroupId)
                .GreaterThan(0)
                .MustAsync(async (tagGroupId, cancellationToken) =>
                {
                    var tagGroup = await tagGroupRepository.GetTagGroupById(tagGroupId, cancellationToken: cancellationToken);

                    return tagGroup is not null;
                })
                .WithMessage("Tag Group with id '{PropertyValue}' does not exist.");
        }
    }

    public class Handler(ITagGroupRepository tagGroupRepository)
        : IRequestHandler<Query, TagGroupDto>
    {
        public async Task<TagGroupDto> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var tagGroup = await tagGroupRepository.GetTagGroupById(
                request.TagGroupId,
                cancellationToken: cancellationToken);

            return new TagGroupDto(tagGroup!.Id, tagGroup.Name);
        }
    }

    public record TagGroupDto(
        int Id,
        string Name);
}