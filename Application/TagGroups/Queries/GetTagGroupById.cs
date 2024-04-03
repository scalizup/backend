using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.TagGroups.Queries;

public static class GetTagGroupById
{
    public record Query(
        int TenantId,
        int TagGroupId) : BasePermissionRequest(TenantId), IRequest<TagGroupDto>;

    public class Validator : AbstractValidator<Query>
    {
        public Validator(ITagGroupRepository tagGroupRepository)
        {
            RuleFor(x => x.TagGroupId)
                .GreaterThan(0)
                .MustAsync(async (tagGroupId, cancellationToken) =>
                {
                    var tagGroup = await tagGroupRepository.GetTagGroupById(tagGroupId, cancellationToken);

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
                cancellationToken);

            return new TagGroupDto(tagGroup!.Id, tagGroup.Name);
        }
    }

    public record TagGroupDto(
        int Id,
        string Name);
}