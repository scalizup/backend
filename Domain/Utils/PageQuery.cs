namespace Domain.Utils;

public class PageQuery
{
    private int _pageNumber;
    public required int PageNumber
    {
        get => _pageNumber;
        set
        {
            if (value < 1)
            {
                throw new InvalidPageNumberException();
            }

            _pageNumber = value;
        }
    }

    private int _pageSize;
    public required int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 1)
            {
                throw new InvalidPageSizeException();
            }

            _pageSize = value;
        }
    }
}