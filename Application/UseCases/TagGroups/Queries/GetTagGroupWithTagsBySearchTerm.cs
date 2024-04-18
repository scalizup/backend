using Application.Repositories;

namespace Application.UseCases.TagGroups.Queries;

public static class GetTagGroupWithTagsBySearchTerm
{
    public record Query(
        string SearchTerm) : BasePermissionRequest, IRequest<IEnumerable<TagGroupDto>>;

    public class Validator : AbstractValidator<Query>
    {
        public Validator(ITagGroupRepository tagGroupRepository)
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty();
        }
    }

    public class Handler(ITagGroupRepository tagGroupRepository)
        : IRequestHandler<Query, IEnumerable<TagGroupDto>>
    {
        public async Task<IEnumerable<TagGroupDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var tagGroups = await tagGroupRepository.GetTagGroupWithTagsBySearchTerm(
                request.TenantId,
                request.SearchTerm.ToLower(),
                cancellationToken);

            return tagGroups.Select(tg => new TagGroupDto(
                tg.Id,
                tg.Name,
                tg.Tags.Select(tag => new TagDto(tag.Id, tag.Name))));
        }
    }

    public record TagDto(
        int Id,
        string Name);

    public record TagGroupDto(
        int Id,
        string Name,
        IEnumerable<TagDto> Tags);
}