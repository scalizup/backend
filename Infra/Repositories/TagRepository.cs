using Application.Models;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class TagRepository(
    AppDbContext context) : ITagRepository
{
    public async Task<Tag?> GetTagById(int id, CancellationToken cancellationToken)
    {
        var tag = await context.Tags
            .FindAsync([id], cancellationToken: cancellationToken);

        return tag;
    }

    public async Task<PageQueryResponse<Tag>> GetAllTags(
        int tagGroupId,
        PageQuery pageQuery,
        CancellationToken cancellationToken)
    {
        var skip = pageQuery.PageSize * (pageQuery.PageNumber - 1);

        var tags = await context.Tags
            .Where(t => t.TagGroupId == tagGroupId)
            .OrderBy(tg => tg.Id)
            .Skip(skip)
            .Take(pageQuery.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalEntities = await context.Tags
            .Where(t => t.TagGroupId == tagGroupId)
            .CountAsync(cancellationToken);

        return new PageQueryResponse<Tag>(
            totalEntities,
            pageQuery,
            tags);
    }

    public async Task<List<Tag>> GetTagsByIds(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var tags = await context.Tags
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(cancellationToken);

        return tags;
    }

    public async Task<int> CreateTag(Tag tag, CancellationToken cancellationToken)
    {
        await context.Tags.AddAsync(tag, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return tag.Id;
    }

    public async Task<bool> UpdateTag(int id, string? name, CancellationToken cancellationToken)
    {
        var tag = await context.Tags
            .FindAsync(id, cancellationToken);

        if (tag is null)
        {
            return false;
        }

        if (name is not null && name != tag.Name)
        {
            tag.Name = name;
        }

        if (context.Entry(tag).State == EntityState.Unchanged)
        {
            return false;
        }

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }

    public async Task<bool> DeleteTag(int id, CancellationToken cancellationToken)
    {
        var tag = await context.Tags
            .FindAsync(new object?[] { id }, cancellationToken: cancellationToken);

        if (tag is null)
        {
            return false;
        }

        context.Tags.Remove(tag);

        var result = await context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}