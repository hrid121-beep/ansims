using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Helpers
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static ServiceResult SuccessResult(string message, object data = null)
        {
            return new ServiceResult { Success = true, Message = message, Data = data };
        }

        public static ServiceResult Failure(string message)
        {
            return new ServiceResult { Success = false, Message = message };
        }
    }

    public static class PaginationHelper
    {
        public static PagedResult<T> CreatePagedResult<T>(
            IQueryable<T> source,
            int pageNumber,
            int pageSize)
        {
            var totalCount = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }

        public static PagedResult<T> CreatePagedResult<T>(
            List<T> items,
            int totalCount,
            int pageNumber,
            int pageSize)
        {
            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
