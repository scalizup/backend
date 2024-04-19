using Application.Common.Exceptions;
using Application.Repositories;

namespace Application.UseCases.Menu.Queries;

public class GetMenuSort 
{
    public record Query : BasePermissionRequest, IRequest<MenuDto>;

    public class Handler(
        IPropertyOrderRepository propertyOrderRepository,
        ITagGroupRepository tagGroupRepository,
        ITagRepository tagRepository) : IRequestHandler<Query, MenuDto>
    {
        public async Task<MenuDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var order = await propertyOrderRepository.GetAsync(request.TenantId, cancellationToken);
            if (order == null)
            {
                return null!;
            }

            var tagGroup = await tagGroupRepository.GetTagGroupById(order.TagGroupId, cancellationToken);
            if (tagGroup == null)
            {
                throw new NotFoundException("The tag group id configured in the order does not exist.");
            }

            var tags = await tagRepository.GetAllTags(tagGroup.Id, new()
            {
                PageNumber = 1,
                PageSize = 100
            }, cancellationToken);

            var tagOrderIndex = order.OrderOfIds
                .Select((id, index) => new { id, index })
                .ToDictionary(x => x.id, x => x.index);

            var result = tags
                .Items
                .OrderBy(t =>
                {
                    if (tagOrderIndex.TryGetValue(t.Id, out var index))
                    {
                        return index;
                    }
                    
                    return tagOrderIndex.Count + 1;
                })
                .Select(t => new TagDto(t.Id, t.Name));

            return new(tagGroup.Name, result);
        }
    }

    public record MenuDto(
        string TagGroupName,
        IEnumerable<TagDto> Tags);

    public record TagDto(
        int Id,
        string Name);
}