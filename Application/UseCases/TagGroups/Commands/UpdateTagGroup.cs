using Application.Common.Exceptions;
using Application.Repositories;

namespace Application.UseCases.TagGroups.Commands;

public static class UpdateTagGroup
{
    public record Command(
        int Id,
        string? Name) : BasePermissionRequest, IRequest<bool>;

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
        }
    }

    public class Handler(ITagGroupRepository tagGroupRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var tagGroup = await tagGroupRepository.GetTagGroupById(request.Id, cancellationToken);
            if (tagGroup is null)
            {
                throw new NotFoundException($"Tag group with id {request.Id} was not found.");
            }

            var wasUpdated = await tagGroupRepository.UpdateTagGroup(
                request.Id,
                request.Name,
                cancellationToken);

            return wasUpdated;
        }
    }
}