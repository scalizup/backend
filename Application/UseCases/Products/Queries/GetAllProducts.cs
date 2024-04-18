using Application.Models;
using Application.Repositories;
using Domain.Entities;

namespace Application.UseCases.Products.Queries;

public static class GetAllProducts
{
    public record Query(
        PageQuery PageQuery) : BasePermissionRequest, IRequest<PageQueryResponse<ProductDto>>;

    public class Handler(
        IProductRepository productRepository) : IRequestHandler<Query, PageQueryResponse<ProductDto>>
    {
        public async Task<PageQueryResponse<ProductDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var response = await productRepository.GetAllProductsAsync(
                request.TenantId,
                request.PageQuery,
                cancellationToken);

            return response.Transform(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.ImageUrl,
                p.Tags.MapToTagGroups()));
        }
    }

    private static IEnumerable<TagGroupDto> MapToTagGroups(this IReadOnlyCollection<Tag> tags)
    {
        var tagGroups = tags
            .DistinctBy(t => t.TagGroupId);

        var tagsOfTagGroups = tagGroups.Select(tg => new TagGroupDto(
            tg.TagGroup.Id,
            tg.TagGroup.Name,
            tags.Where(t => t.TagGroupId == tg.TagGroupId).Select(t => new TagDto(t.Id, t.Name))));

        return tagsOfTagGroups;
    }

    public record ProductDto(
        int Id,
        string Name,
        string? Description,
        decimal? Price,
        string? ImageUrl,
        IEnumerable<TagGroupDto> TagGroups);

    public record TagGroupDto(
        int Id,
        string Name,
        IEnumerable<TagDto> Tags);

    public record TagDto(
        int Id,
        string Name);
}