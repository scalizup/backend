using Application.Common.Exceptions;
using Application.Repositories;

namespace Application.UseCases.Menu.Queries;

public class GetMenuSortedByOrderName
{
    public record Query() : BasePermissionRequest, IRequest<MenuDto>;

    public class Handler(
        IPropertyOrderRepository propertyOrderRepository,
        ITagGroupRepository tagGroupRepository,
        ITagRepository tagRepository,
        IProductRepository productRepository) : IRequestHandler<Query, MenuDto>
    {
        public async Task<MenuDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var order = await propertyOrderRepository.GetAsync(request.TenantId, cancellationToken);

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

            var products = await productRepository
                .GetProductsByTagIds(tags.Items.Select(t => t.Id), cancellationToken);

            var tagOrderIndex = order.OrderOfIds
                .Select((id, index) => new { id, index })
                .ToDictionary(x => x.id, x => x.index);

            return new(tagGroup.Name, tags.Items
                .OrderBy(t =>
                {
                    if (tagOrderIndex.TryGetValue(t.Id, out var index))
                    {
                        return index;
                    }
                    
                    return tagOrderIndex.Count + 1;
                })
                .Select(t => new MenuItemDto(t.Name, products
                    .Where(p => p.Tags.Select(t => t.Id).Contains(t.Id))
                    .Select(p => new MenuItemProductDto(p.Name, p.Description, p.Price, p.ImageUrl, p.Tags.Select(t => t.Name))))));
        }
    }

    public record MenuDto(
        string TagGroupName,
        IEnumerable<MenuItemDto> MenuItems);

    public record MenuItemDto(
        string Name,
        IEnumerable<MenuItemProductDto?> Products);

    public record MenuItemProductDto(
        string Name,
        string? Description,
        decimal? Price,
        string?ImageUrl,
        IEnumerable<string> Tags);
}