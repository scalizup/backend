using Application.Models;
using Domain.Entities;

namespace Application.Repositories;

public class TagGroupFilter
{
    public bool IncludeTags { get; set; } = false;
}

public interface ITagGroupRepository
{
    Task<TagGroup?> GetTagGroupById(int id, TagGroupFilter? filter = default, CancellationToken cancellationToken = default);

    Task<IEnumerable<TagGroup>> GetTagGroupWithTagsBySearchTerm(
        int tenantId,
        string searchTerm,
        CancellationToken cancellationToken);

    Task<PageQueryResponse<TagGroup>> GetAllTagGroups(
        int tenantId,
        PageQuery pageQuery,
        CancellationToken cancellationToken);

    Task<int> CreateTagGroup(TagGroup tagGroup, CancellationToken cancellationToken);

    Task<bool> UpdateTagGroup(int id, string? name, CancellationToken cancellationToken);

    Task<bool> DeleteTagGroup(int id, CancellationToken cancellationToken);

    Task<PageQueryResponse<TagGroup>> GetAllTagGroupsWithTags(int tenantId, PageQuery pageQuery,
        CancellationToken cancellationToken);
}