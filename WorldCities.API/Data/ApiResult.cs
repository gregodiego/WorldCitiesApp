        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Reflection;
        using System.Threading.Tasks;
        using Microsoft.EntityFrameworkCore;
        using System.Linq.Dynamic.Core;

        namespace WorldCities.API.Data
        {
            public class ApiResult<T>
            {
                public List<T> Data { get; private set; }
        public ApiResult(int count, int pageIndex, int pageSize, int totalCount, int totalPages, string sortColumn, string sortOrder) 
        {
            this.Count = count;
                this.PageIndex = pageIndex;
                this.PageSize = pageSize;
                this.TotalCount = totalCount;
                this.TotalPages = totalPages;
                this.SortColumn = sortColumn;
                this.SortOrder = sortOrder;
               
        }
                        public int Count { get; private set; }
                public int PageIndex { get; private set; }
                public int PageSize { get; private set; }
                public int TotalCount { get; private set; }
                public int TotalPages { get; private set; }
                public string SortColumn { get; set; }
                public string SortOrder { get; set; }

                public bool HasPreviousPage { get{
                    return (PageIndex > 0);
                } }

                public bool HasNextPage { get{
                    return ((PageIndex + 1) < TotalPages);
                } }
                private ApiResult(List<T> data, int count, int pageIndex, int pageSize, string sortColumn, string sortOrder){
                    Data = data;
                    Count = count;
                    PageIndex = pageIndex;
                    PageSize = PageSize;
                    TotalCount = count;
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                    SortColumn = sortColumn;
                    SortOrder = sortOrder;
                }

                public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, 
                                                                    string sortColumn = null, string sortOrder = null){
                var count = await source.CountAsync();
                if (!string.IsNullOrEmpty(sortColumn)&& IsValidProperty(sortColumn))
                {
                    sortOrder = !string.IsNullOrEmpty(sortOrder)
                    && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
                    source = source.OrderBy(string.Format( "{0} {1}", sortColumn, sortOrder)); 
                }
                source = source
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
                var data = await source.ToListAsync();
                return new ApiResult<T>(
                    data,
                    count,
                    pageIndex,
                    pageSize,
                    sortColumn,
                    sortOrder);
                }

                public static bool IsValidProperty( string propertyName,bool throwExceptionIfNotFound = true)
                {
                var prop = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                    BindingFlags.Public |
                    BindingFlags.Instance);
                if (prop == null && throwExceptionIfNotFound)
                    throw new NotSupportedException(
                        string.Format(
                            "ERROR: Property '{0}' does not exist.",
                            propertyName)
                        );
                return prop != null;
                }
            }
        }