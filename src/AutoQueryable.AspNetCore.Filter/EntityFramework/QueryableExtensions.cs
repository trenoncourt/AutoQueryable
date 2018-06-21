using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoQueryable.Core.Models;
using AutoQueryable.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoQueryable.AspNetCore.Filter.EntityFramework
{
    public static class QueryableExtensions
    {
        // TODO: Implement WrapWith
        public static async Task<dynamic> ToAutoQueryListResultAsync<TEntity>(this IQueryable<TEntity> query, IAutoQueryableContext context) where TEntity : class
        {
            var result = await query.ToListAsyncSafe();

            //if (!context.ClauseValueManager.Page.HasValue || context.ClauseValueManager.Page.Value == 0)
            //{
            //    return result;
            //}
            
            return new PagedResult<TEntity>
            {
                TotalCount = context.TotalCountQuery != null ? await context.TotalCountQuery.CountAsyncSafe() : result.Count,
                RowCount = result.Count,
                Result = result
            };
        }
    }
    public static class EfExtensions
    {
        public static Task<List<TSource>> ToListAsyncSafe<TSource>(
            this IQueryable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source is IAsyncEnumerable<TSource>))
                return Task.FromResult(source.ToList());
            return source.ToListAsync();
        }
        public static Task<int> CountAsyncSafe<TSource>(
            this IQueryable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!(source is IAsyncEnumerable<TSource>))
                return Task.FromResult(source.Count());
            return source.CountAsync();
        }
    }
}
