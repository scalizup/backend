using Application.Common.Exceptions;
using Application.Repositories;
using Domain.Entities.Menu;

namespace Application.UseCases.MenuSort.Commands;

public static class UpsertMenuSort
{
    public record Command(
        int TagGroupId,
        List<OrderDto> Orders) : BasePermissionRequest, IRequest<int>;

    public record OrderDto(
        int TagId,
        List<int> ProductIds);

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.TagGroupId)
                .NotEmpty();
        }
    }

    public class Handler(
        IMenuSortRepository menuSortRepository,
        ITagGroupRepository tagGroupRepository,
        ITagRepository tagRepository,
        IProductRepository productRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var tagGroup = await tagGroupRepository.GetTagGroupById(request.TagGroupId, new() { IncludeTags = true },
                cancellationToken);
            if (tagGroup is null)
            {
                throw new NotFoundException("Tag group not found");
            }

            // Check if all the tags in the order exist
            foreach (var order in request.Orders)
            {
                var tag = await tagRepository.GetTagById(order.TagId, cancellationToken);
                if (tag is null)
                {
                    throw new NotFoundException("Tag not found");
                }
                
                // Get the products associated with the tag
                var products = await productRepository.GetProductsByTagId(order.TagId, cancellationToken);
                if (!products.Any())
                {
                    throw new NotFoundException("Products not found");
                }
                
                // Check if all the products in the order exist
                var productIds = products.Select(p => p.Id).ToList();
                var missingProductIds = order.ProductIds.Except(productIds).ToList();
                if (missingProductIds.Count != 0)
                {
                    throw new NotFoundException("Products not found");
                }
                
                // Add the missing tags to the last positions
                request.Orders.Add(order with { ProductIds = missingProductIds });
            }

            // Check if all the tags in the tag group are present in the request
            if (tagGroup.Tags.Count() != request.Orders.Count)
            {
                var missingTagIds = tagGroup.Tags
                    .Select(t => t.Id)
                    .Except(request.Orders.Select(o => o.TagId))
                    .ToList();

                foreach (var missingTagId in missingTagIds)
                {
                    var products = await productRepository.GetProductsByTagId(missingTagId, cancellationToken);
                    // Add the missing tags to the last positions
                    request.Orders.Add(new OrderDto(missingTagId, products.Select(p => p.Id).ToList()));
                }
            }

            var menuSort = new Domain.Entities.Menu.MenuSort(
                request.TenantId)
            {
                TagGroupId = request.TagGroupId,
                ProductsTagOrders = request.Orders.Select(dto => new ProductsTagOrder
                {
                    TagId = dto.TagId,
                    ProductsIds = dto.ProductIds.ToList(),
                }).ToList()
            };

            var result = await menuSortRepository.UpsertAsync(menuSort, cancellationToken);

            return result;
        }
    }
}