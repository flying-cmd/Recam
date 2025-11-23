namespace Remp.Common.Helpers;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext;
    public bool HasPrevious;

    public PagedResult(int pageNumber, int pageSize, int totalCount, IEnumerable<T> items)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNext = PageNumber + 1 <= TotalPages;
        HasPrevious = PageNumber > 1;
    }
}
