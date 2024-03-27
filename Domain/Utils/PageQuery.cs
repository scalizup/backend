namespace Domain.Utils;

public record PageQuery
{
    public PageQuery(
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageNumber < 1)
        {
            throw new InvalidPageNumberException();
        }

        if (pageSize < 1)
        {
            throw new InvalidPageSizeException();
        }
        
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; }

    public int PageSize { get; }
}