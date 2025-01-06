namespace TodoList.Application.Core;

public class PaginationInfo
{
    private const int DefaultPageSize = 50;

    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int Start { get; private set; }

    private PaginationInfo(int page, int pageSize)
    {
        Page = Math.Max(1, page);
        PageSize = pageSize > 0 ? pageSize : DefaultPageSize;
        Start = (Page - 1) * PageSize;
    }

    public static PaginationInfo Create(int page, int pageSize)
    {
        return new PaginationInfo(page, pageSize);
    }

    public int CalculateTotalPages(int totalItems)
    {
        return (int)Math.Ceiling((double)totalItems / PageSize);
    }
}
