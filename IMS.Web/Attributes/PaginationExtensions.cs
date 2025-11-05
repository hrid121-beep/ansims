using IMS.Application.DTOs;
using IMS.Application.Helpers;

namespace IMS.Web.Extensions
{
    public static class PaginationExtensions
    {
        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, int page, int pageSize)
        {
            var totalCount = source.Count();
            var items = source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
                // TotalPages, HasPrevious, and HasNext are computed automatically
            };
        }
    }
}