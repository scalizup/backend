using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Entities;

namespace Application.UseCases.MenuSort.Queries;

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

            var tagGroup = await tagGroupRepository.GetTagGroupById(
                existingMenuSort.TagGroupId,
                new() { IncludeTags = true },
                cancellationToken);

            var products = await productRepository.GetProductsByIds(
                existingMenuSort.ProductsTagOrders.SelectMany(pto => pto.ProductsIds),
                cancellationToken);

            var sortedProducts = SortProductsByCustomOrder(
                existingMenuSort,
                products,
                tagGroup!);

            return new MenuDto(
                tagGroup!.Name,
                sortedProducts.Keys.Select(tagName => new TagDto(
                    tagName,
                    sortedProducts[tagName].Select(p => new ProductDto(
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.ImageUrl)))));
        }
    }

    public record MenuDto(
        string TagGroupName,
        IEnumerable<TagDto> Tags);

    public record TagDto(
        string Name,
        IEnumerable<ProductDto> Products);

    public record ProductDto(
        int Id,
        string Name,
        string? Description,
        decimal? Price,
        string? ImageUrl);

    public static Dictionary<string, List<Product>> SortProductsByCustomOrder(
        Domain.Entities.Menu.MenuSort menuSort,
        IEnumerable<Product> products,
        TagGroup tagGroup)
    {
        var sortedProducts = new Dictionary<string, List<Product>>();

        foreach (var order in menuSort.ProductsTagOrders)
        {
            var tag = tagGroup.Tags.FirstOrDefault(t => t.Id == order.TagId);
            if (tag != null)
            {
                var productsInTagGroup = products.Where(p => p.TagIds.Contains(tag.Id)).ToList();
                var productsInTag = productsInTagGroup.Where(p => p.TagIds.Contains(tag.Id)).ToList();

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

        return sortedProducts;
    }
}