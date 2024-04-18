using Application.Models;
using Application.Repositories;

namespace Application.UseCases.TagGroups.Queries;

public static class GetAllTagGroupsWithTags
{
    public record Query(
        PageQuery PageQuery) : BasePermissionRequest, IRequest<PageQueryResponse<TagGroupDto>>;

    public class Handler(ITagGroupRepository tagGroupRepository)
        : IRequestHandler<Query, PageQueryResponse<TagGroupDto>>
    {
        public async Task<PageQueryResponse<TagGroupDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var response = await tagGroupRepository.GetAllTagGroupsWithTags(
                request.TenantId,
                request.PageQuery,
                cancellationToken);

            return response.Transform(t => new TagGroupDto(
                t.Id,
                t.Name,
                t.Tags.Select(tag => new TagDto(tag.Id, tag.Name))));
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