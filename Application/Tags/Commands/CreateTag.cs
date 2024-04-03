using Application.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Tags.Commands;

public static class CreateTag
{
    public record Command(
        int TenantId,
        int TagGroupId,
        string Name) : BasePermissionRequest(TenantId), IRequest<int>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator(
            ITenantRepository tenantRepository,
            ITagGroupRepository tagGroupRepository)
        {
            RuleFor(x => x.TenantId)
                .GreaterThan(0)
                .MustAsync(async (tenantId, cancellationToken) =>
                {
                    var tenant = await tenantRepository.GetTenantById(tenantId, cancellationToken);

                    return tenant is not null;
                })
                .WithMessage("Tenant with id '{PropertyValue}' does not exist.");

            RuleFor(x => x.TagGroupId)
                .GreaterThan(0)
                .MustAsync(async (tagGroupId, cancellationToken) =>
                {
                    var tagGroup = await tagGroupRepository.GetTagGroupById(tagGroupId, cancellationToken);

                    return tagGroup is not null;
                })
                .WithMessage("Tag Group with id '{PropertyValue}' does not exist.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2);
        }
    }

    public class Handler(ITagRepository tagRepository) : IRequestHandler<Command, int>
    {
        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var tagGroup = new Tag(
                request.TenantId,
                request.TagGroupId,
                request.Name);

            var newTag = await tagRepository.CreateTag(tagGroup, cancellationToken);

            return newTag;
        }
    }
}