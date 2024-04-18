using Application.Common.Interfaces;
using Application.Repositories;
using Domain.Entities;

namespace Application.UseCases.Products.Commands;

public static class CreateProduct
{
    public record Command(
        string Name,
        IEnumerable<int> TagIds,
        string? Description = null,
        decimal? Price = null) : BasePermissionRequest, IRequest<int>
    {
        public IImage? Image { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(c => c.TagIds)
                .NotEmpty()
                .ForEach(t => t.GreaterThan(0));

            RuleFor(c => c.Description)
                .MaximumLength(500)
                .When(c => c.Description is not null);

            RuleFor(c => c.Price)
                .GreaterThan(0)
                .When(c => c.Price is not null);

            RuleFor(c => c.Image)
                .ChildRules(image =>
                {
                    image.RuleFor(i => i!.FileName)
                        .Matches(@"\.(jpg|jpeg|png)$")
                        .NotEmpty()
                        .WithMessage("Image must be a valid file type (jpg, jpeg, png)");

                    image.RuleFor(i => i!.Length)
                        .LessThanOrEqualTo(10 * 1024 * 1024)
                        .WithMessage("Image size must be less than 10MB");
                })
                .When(c => c.Image is not null);
        }
    }

    public class Handler(
        IProductRepository productRepository,
        ITagRepository tagRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingTags = await tagRepository.GetTagsByIds(request.TagIds, cancellationToken);

            if (existingTags.Count != request.TagIds.Count())
            {
                throw new Exception("One or more tags do not exist");
            }

            var product = new Product(
                request.TenantId,
                request.Name)
            {
                Price = request.Price,
                Description = request.Description,
                Tags = existingTags
            };

            if (request.Image is not null)
            {
                var filePath = await request.Image.CopyToAsync(cancellationToken);

                product.ImageUrl = filePath;
            }

            return await productRepository.CreateProductAsync(product, cancellationToken);
        }
    }
}