using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Repositories;

namespace Application.UseCases.Products.Commands;

public static class UpdateProduct
{
    public record Command(
        int Id,
        string? Name,
        IEnumerable<int>? TagIds,
        string? Description,
        decimal? Price) : BasePermissionRequest, IRequest<bool>
    {
        public IImage? Image { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0);

            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(2)
                .When(t => t.Name is not null);

            RuleFor(c => c.TagIds)
                .NotEmpty()
                .ForEach(t => t.GreaterThan(0))
                .When(c => c.TagIds is not null);

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
        ITagRepository tagRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingProduct = await productRepository.GetProductByIdAsync(request.Id, cancellationToken);
            if (existingProduct is null)
            {
                throw new NotFoundException($"Tag group with id {request.Id} was not found.");
            }

            if (request.Name is not null && request.Name != existingProduct.Name)
            {
                existingProduct.Name = request.Name;
            }

            if (request.Description != existingProduct.Description)
            {
                existingProduct.Description = request.Description;
            }

            if (request.Price != existingProduct.Price)
            {
                existingProduct.Price = request.Price;
            }

            if (request.TagIds is not null && !request.TagIds.SequenceEqual(existingProduct.Tags.Select(t => t.Id)))
            {
                var existingTagIds = new HashSet<int>(existingProduct.Tags.Select(t => t.Id).Order());
                var requestTagIds = new HashSet<int>(request.TagIds.Order());

                if (!requestTagIds.SetEquals(existingTagIds))
                {
                    var tags = await tagRepository.GetTagsByIds(request.TagIds, cancellationToken);
                    if (tags.Count != request.TagIds.Count())
                    {
                        throw new NotFoundException("One or more tags were not found.");
                    }

                    existingProduct.Tags = tags;
                }
            }

            if (request.Image is not null && request.Image.FileName != existingProduct.ImageUrl)
            {
                var filePath = await request.Image.CopyToAsync(cancellationToken);

                existingProduct.ImageUrl = filePath;
            }

            var wasUpdated = await productRepository.UpdateProductAsync(
                existingProduct,
                cancellationToken);

            return wasUpdated;
        }
    }
}