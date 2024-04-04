using Application.Models;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.TagGroups.Queries;

public static class GetAllTagGroups
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
            var response = await tagGroupRepository.GetAllTagGroups(
                request.TenantId,
                request.PageQuery,
                cancellationToken);

            return response.Transform(t => new TagGroupDto(t.Id, t.Name));
        }
    }

    public record TagGroupDto(
        int Id,
        string Name);
}