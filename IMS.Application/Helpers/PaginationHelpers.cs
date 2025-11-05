// Add this to your IMS.Application/Helpers/PaginationHelpers.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace IMS.Application.Helpers
{
    public class PagedResult<T>
    {
        private List<T> _items = new List<T>();

        // Smart property: accepts IEnumerable<T>, always stores as List<T>
        public IEnumerable<T> Items
        {
            get => _items;
            set => _items = (value ?? Enumerable.Empty<T>()).ToList();
        }

        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Pagination helpers
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        // Default constructor
        public PagedResult()
        {
            _items = new List<T>();
            TotalCount = 0;
            PageNumber = 1;
            PageSize = 20; // Default page size
        }

        // Constructor with IEnumerable<T>
        public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            _items = (items ?? Enumerable.Empty<T>()).ToList();
            TotalCount = totalCount;
            PageNumber = pageNumber > 0 ? pageNumber : 1;
            PageSize = pageSize > 0 ? pageSize : 20;
        }
    }

    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string SearchTerm { get; set; }
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
    }

    public static class QueryableExtensions
    {
        public static PagedResult<T> ToPagedResult<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            var count = query.Count();
            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }
    }
}