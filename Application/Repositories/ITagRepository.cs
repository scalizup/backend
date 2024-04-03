using Application.Models;
using Domain.Entities;

namespace Application.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetTagById(int id, CancellationToken cancellationToken);

    Task<PageQueryResponse<Tag>> GetAllTags(
        int tagGroupId,
        PageQuery pageQuery,
        CancellationToken cancellationToken);

    Task<int> CreateTag(Tag tag, CancellationToken cancellationToken);

    Task<bool> UpdateTag(int id, string? name, CancellationToken cancellationToken);

    Task<bool> DeleteTag(int id, CancellationToken cancellationToken);
}