using TodoList.Application.Core;
using TodoList.Application.DTOs.Shared;

namespace TodoList.Application.Factories;
public static class PagedResultFactory
{
    public static PagedResultDTo<T> CreatePagedResult<T>(
        IEnumerable<T> items, int totalItems, PaginationInfo paginationInfo)
    {
        return new PagedResultDTo<T>
        {
            Items = items ?? [],
            Page = paginationInfo.Page,
            PageSize = paginationInfo.PageSize,
            TotalPages = paginationInfo.CalculateTotalPages(totalItems),
            TotalItems = totalItems
        };
    }
}
