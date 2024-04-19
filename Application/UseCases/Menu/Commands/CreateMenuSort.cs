using Application.Repositories;
using Domain.Entities.Menu;

namespace Application.UseCases.Menu.Commands;

public static class CreateMenuSort
{
    public record Command(
        int TagGroupId,
        List<int> OrderOfTagIds) : BasePermissionRequest, IRequest<int>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.TagGroupId)
                .NotEmpty();

            RuleFor(x => x.OrderOfTagIds)
                .NotEmpty();
        }
    }

    public class Handler(IPropertyOrderRepository propertyOrderRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var propertyOrder = new PropertyOrder(
                request.TenantId,
                request.TagGroupId,
                request.OrderOfTagIds);

            var newPropertyOrder = await propertyOrderRepository.UpsertPropertyOrderAsync(propertyOrder, cancellationToken);

            return newPropertyOrder;
        }
    }
}