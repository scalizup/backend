using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Entities;

namespace Application.UseCases.Menu.Queries;

public class GetMenuSort 
{
    public record Query : BasePermissionRequest, IRequest<MenuDto>;

    public class Handler(
        IMenuSortRepository menuSortRepository,
        ITagGroupRepository tagGroupRepository,
        ITagRepository tagRepository,
        IProductRepository productRepository) : IRequestHandler<Query, MenuDto>
    {
        public async Task<MenuDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var existingMenuSort = await menuSortRepository.GetAsync(request.TenantId, cancellationToken);
            if (existingMenuSort is null)
            {
                throw new NotFoundException("Menu sort not found");
            }
            
            var tagGroupWithTags = await tagGroupRepository.GetTagGroupById(
                existingMenuSort.TagGroupId,
                new() { IncludeTags = true },
                cancellationToken);
            
            var products = await productRepository.GetProductsByTagIds(
                existingMenuSort.ProductsTagOrders.Select(pto => pto.TagId),
                cancellationToken);
            
            var sortedProducts = SortProductsByCustomOrder(
                existingMenuSort,
                products,
                new[] { tagGroupWithTags },
                tagGroupWithTags.Tags);
            
            return null!;
        }
    }

    public record MenuDto(
        string TagGroupName,
        IEnumerable<TagDto> Tags);

    public record TagDto(
        int Id,
        string Name);
    
    public static Dictionary<string, List<Product>> SortProductsByCustomOrder(
        Domain.Entities.Menu.MenuSort menuSort,
        IEnumerable<Product> products,
        IEnumerable<TagGroup> tagGroups,
        IEnumerable<Tag> tags)
    {
        var sortedProducts = new Dictionary<string, List<Product>>();

        var tagGroup = tagGroups.FirstOrDefault(tg => tg.Id == menuSort.TagGroupId);
        if (tagGroup != null)
        {
            foreach (var order in menuSort.ProductsTagOrders)
            {
                var tag = tags.FirstOrDefault(t => t.Id == order.TagId && t.TagGroupId == menuSort.TagGroupId);
                if (tag != null)
                {
                    var productsInTagGroup = products.Where(p => tags.Any(t => t.TagGroupId == tagGroup.Id && p.Tags.Select(p => p.Id).Contains(t.Id))).ToList();
                    var productsInTag = productsInTagGroup.Where(p => p.Tags.Select(p => p.Id).Contains(tag.Id)).ToList();

                    if (productsInTag.Count > 0)
                    {
                        var orderedProducts = new List<Product>();
                        foreach (var productId in order.ProductsIds)
                        {
                            var product = productsInTag.FirstOrDefault(p => p.Id == productId);
                            if (product != null)
                            {
                                orderedProducts.Add(product);
                            }
                        }
                        sortedProducts[tag.Name] = orderedProducts;
                    }
                }
            }
        }

        return sortedProducts;
    }
}
