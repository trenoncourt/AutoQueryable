using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.Clauses;
using Microsoft.EntityFrameworkCore;

namespace AutoQueryable.Extensions
{
    public static class QueryableExtension
    {
        //public static IQueryable<object> AutoQueryable(this IQueryable<object> query, IAutoQueryableContext context) => context.GetAutoQuery(query);

        public static IQueryable<dynamic> AutoQueryable<TEntity>(this IQueryable<TEntity> query, IAutoQueryableContext context) where TEntity : class => context.GetAutoQuery(query);

        public static IQueryable<T> Call<T>(this IQueryable<T> source, string method, string propertyName)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            Expression lambda = Expression.Lambda(propertyAccess, parameter);
            var resultExp = Expression.Call(typeof(Queryable), method, new[] { typeof(T), property.PropertyType }, source.Expression, lambda);

            return source.Provider.CreateQuery<T>(resultExp);
        }
        public static IPagedResult<TEntity> ToPagedResult<TEntity>(this IQueryable<TEntity> query, IAutoQueryableContext context) where TEntity : class
        {
            var result = query.ToList();
            return new PagedResult<TEntity>
            {
                TotalCount = context.TotalCountQuery?.Count() ?? result.Count,
                RowCount = result.Count,
                Result = result
            };
        }
        public static async Task<IPagedResult<TEntity>> ToPagedResultAsync<TEntity>(this IQueryable<TEntity> query, IAutoQueryableContext context) where TEntity : class
        {
            var result = await query.ToListAsyncSafe();
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
