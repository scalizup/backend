using Application.Common.Exceptions;
using Application.Repositories;

namespace Application.UseCases.Products.Commands;

public static class DeleteProduct
{
    public record Command(
        int Id) : BasePermissionRequest, IRequest<bool>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }

    public class Handler(IProductRepository productRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingProduct = await productRepository.GetProductByIdAsync(request.Id, cancellationToken);
            if (existingProduct is null)
            {
                throw new NotFoundException($"Product with id {request.Id} was not found.");
            }
            
            var wasDeleted = await productRepository.DeleteProductAsync(request.Id, cancellationToken);
            
            return wasDeleted;
        }
    }
}