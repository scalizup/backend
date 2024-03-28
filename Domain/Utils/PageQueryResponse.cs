namespace Domain.Utils;

public class PageQueryResponse<TEntity>(int totalItems, PageQuery pageQuery, IEnumerable<TEntity> items)
    where TEntity : class
{
    public int CurrentPage { get; init; } = pageQuery.PageNumber;

    public int PageSize { get; init; } = pageQuery.PageSize;

    public int TotalItems { get; init; } = totalItems;

    public int TotalPages { get; } = (int)Math.Ceiling((double)totalItems / pageQuery.PageSize);
    
    public IEnumerable<TEntity> Items { get; set; } = items;

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

    public PageQueryResponse<TEntityDto> Transform<TEntityDto>(Func<TEntity, TEntityDto> func)
        where TEntityDto : class
    {
        return new PageQueryResponse<TEntityDto>(
            TotalItems,
            new PageQuery
            {
                PageNumber = CurrentPage,
                PageSize =  pageQuery.PageSize
            },
            Items.Select(func));
    }
}