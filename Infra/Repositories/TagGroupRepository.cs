using Application.Models;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class TagGroupRepository(
    AppDbContext context) : ITagGroupRepository
{
    public Task<TagGroup?> GetTagGroupById(int id, CancellationToken cancellationToken)
    {
        var tagGroup = context.TagGroups
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return tagGroup;
    }

    public async Task<PageQueryResponse<TagGroup>> GetAllTagGroups(
        int tenantId,
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        var skip = pageQuery.PageSize * (pageQuery.PageNumber - 1);

        var tagGroups = await context.TagGroups
            .Where(tg => tg.TenantId == tenantId)
            .OrderBy(tg => tg.Id)
            .Skip(skip)
            .Take(pageQuery.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalEntities = await context.TagGroups
            .Where(tg => tg.TenantId == tenantId)
            .CountAsync(cancellationToken);

        return new PageQueryResponse<TagGroup>(
            totalEntities,
            pageQuery,
            tagGroups);
    }

    public async Task<int> CreateTagGroup(TagGroup tagGroup, CancellationToken cancellationToken)
    {
        context.TagGroups.Add(tagGroup);

        await context.SaveChangesAsync(cancellationToken);

        return tagGroup.Id;
    }

    public async Task<bool> UpdateTagGroup(int id, string? name, CancellationToken cancellationToken)
    {
        var tagGroup = await context.TagGroups
            .FindAsync(id, cancellationToken);

        if (tagGroup is null)
        {
            return false;
        }

        if (name is not null && name != tagGroup.Name)
        {
            tagGroup.Name = name;
        }

        if (context.Entry(tagGroup).State == EntityState.Unchanged)
        {
            return false;
        }

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }

    public async Task<bool> DeleteTagGroup(int id, CancellationToken cancellationToken)
    {
        var tagGroup = await context.TagGroups
            .FindAsync(id);

        if (tagGroup is null)
        {
            return false;
        }

        context.TagGroups.Remove(tagGroup);

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
    
    public async Task<IEnumerable<TagGroup>> GetTagGroupWithTagsBySearchTerm(
        int tenantId,
        string searchTerm,
        CancellationToken cancellationToken)
    {
        var tagGroups = await context.TagGroups
            .Include(tg => tg.Tags)
            .Where(tg =>
                    tg.TenantId == tenantId
                    && tg.Tags.Any()
                    && tg.Name.ToLower().Contains(searchTerm) ||
                    tg.Tags.Any(t => t.Name.ToLower().Contains(searchTerm)))
            .ToListAsync(cancellationToken);

        return tagGroups;
    }

    public async Task<PageQueryResponse<TagGroup>> GetAllTagGroupsWithTags(
        int tenantId,
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        var skip = pageQuery.PageSize * (pageQuery.PageNumber - 1);

        var tagGroups = await context.TagGroups
            .Where(tg => tg.TenantId == tenantId && tg.Tags.Any())
            .OrderBy(tg => tg.Id)
            .Skip(skip)
            .Take(pageQuery.PageSize)
            .Include(tg => tg.Tags)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalEntities = await context.TagGroups
            .Where(tg => tg.TenantId == tenantId)
            .CountAsync(cancellationToken);

        return new PageQueryResponse<TagGroup>(
            totalEntities,
            pageQuery,
            tagGroups);
    }
}