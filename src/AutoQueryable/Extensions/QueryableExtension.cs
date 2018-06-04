using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.Clauses;

namespace AutoQueryable.Extensions
{
    public static class QueryableExtension
    {
        public static IQueryable<object> AutoQueryable(this IQueryable<object> query, IAutoQueryableContext context) => context.GetAutoQuery(query);

        public static IQueryable<TAs> AutoQueryable<TEntity,TAs>(this IQueryable<TEntity> query, IAutoQueryableContext<TEntity,TAs> context) where TEntity : class where TAs : class => context.GetAutoQuery(query);

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
        public static IPagedResult<TEntity> ToPagedResult<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return new PagedResult<TEntity>
            {

            };
        }
    }
}
