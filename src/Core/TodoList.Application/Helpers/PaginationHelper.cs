using TodoList.Application.Core;
using TodoList.Application.DTOs.Shared;
using TodoList.Application.Factories;
namespace TodoList.Application.Helpers;
public static class PaginationHelper
{
    public static PaginationInfo CalculatePagination(int page, int pageSize)
    {
        return PaginationInfo.Create(page, pageSize);
    }
    public static PagedResultDTo<T> CreatePagedResult<T>(
        IEnumerable<T> items, int totalItems, PaginationInfo paginationInfo)
    {
        return PagedResultFactory.CreatePagedResult(items, totalItems, paginationInfo);
    }
}