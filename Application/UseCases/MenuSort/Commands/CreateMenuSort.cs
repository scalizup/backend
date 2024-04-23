using Application.Repositories;
using Domain.Entities.Menu;

namespace Application.UseCases.MenuSort.Commands;

public static class CreateMenuSort
{
    public record Command(
        int TagGroupId,
        List<OrderDto> Orders) : BasePermissionRequest, IRequest<int>;

    public record OrderDto(
        int TagId,
        IEnumerable<int> ProductIds);

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.TagGroupId)
                .NotEmpty();
        }
    }

    public class Handler(IMenuSortRepository menuSortRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
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