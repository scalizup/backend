namespace Domain.UseCases.TagGroup.Queries;

public static class GetAllTagGroups
{
    public record Query(
        int TenantId,
        PageQuery PageQuery) : IRequest<PageQueryResponse<TagGroupDto>>;

    public class Validator : AbstractValidator<Query>
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
        }
    }

    public class Handler(ITagGroupRepository tagGroupRepository)
        : IRequestHandler<Query, PageQueryResponse<TagGroupDto>>
    {
        public async Task<PageQueryResponse<TagGroupDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var response = await tagGroupRepository.GetAllTagGroups(
                request.TenantId,
                request.PageQuery,
                cancellationToken);

            return response.Transform(t => new TagGroupDto(t.Id, t.Name));
        }
    }

    public record TagGroupDto(
        int Id,
        string Name);
}