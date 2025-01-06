namespace TodoList.Application.DTOs.Shared;

public class PagedResultDTo<T>
{
    public required IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
}
