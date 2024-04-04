using Application.Models;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Tags.Queries;

public static class GetAllTags
{
    public record Query(
        int TagGroupId,
        PageQuery PageQuery) : IRequest<PageQueryResponse<TagDto>>;

    public class Validator : AbstractValidator<Query>
    {
        public Validator(ITagGroupRepository tagGroupRepository)
        {
            RuleFor(x => x.TagGroupId)
                .GreaterThan(0)
                .MustAsync(async (tagGroupId, cancellationToken) =>
                {
                    var tagGroup = await tagGroupRepository.GetTagGroupById(tagGroupId, cancellationToken);

                    return tagGroup is not null;
                })
                .WithMessage("Tag Group with id '{PropertyValue}' does not exist.");
        }
    }

    public class Handler(ITagRepository tagRepository)
        : IRequestHandler<Query, PageQueryResponse<TagDto>>
    {
        public async Task<PageQueryResponse<TagDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var response = await tagRepository.GetAllTags(
                request.TagGroupId,
                request.PageQuery,
                cancellationToken);

            return response.Transform(t => new TagDto(t.Id, t.Name));
        }
    }

    public record TagDto(
        int Id,
        string Name);
}