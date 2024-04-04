using Application.Common.Exceptions;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.UseCases.TagGroups.Commands;

public static class DeleteTagGroup
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

    public class Handler(ITagGroupRepository tagGroupRepository) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingTagGroup = await tagGroupRepository.GetTagGroupById(request.Id, cancellationToken);
            if (existingTagGroup is null)
            {
                throw new NotFoundException($"Tag group with id {request.Id} was not found.");
            }
            
            var wasDeleted = await tagGroupRepository.DeleteTagGroup(request.Id, cancellationToken);
            
            return wasDeleted;
        }
    }
}