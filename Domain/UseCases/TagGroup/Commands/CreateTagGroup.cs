namespace Domain.UseCases.TagGroup.Commands;

public static class CreateTagGroup
{
    public record Command(
        int TenantId,
        string Name) : IRequest<int>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITenantRepository tenantRepository)
        {
            RuleFor(x => x.TenantId)
                .GreaterThan(0)
                .MustAsync(async (tenantId, cancellationToken) =>
                {
                    var tenant = await tenantRepository.GetTenantById(tenantId, cancellationToken);

                    return tenant is not null;
                })
                .WithMessage("Tenant with id '{PropertyValue}' does not exist.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(ITagGroupRepository tagGroupRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var tagGroup = new Entities.TagGroup(
                request.TenantId,
                request.Name);

            var newTagGroup = await tagGroupRepository.CreateTagGroup(tagGroup, cancellationToken);

            return newTagGroup;
        }
    }
}