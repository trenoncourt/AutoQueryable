using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoQueryable.Core.Models;
using AutoQueryable.Helpers;

namespace AutoQueryable.Extensions
{
    public static class QueryableExtension
    {
        public static dynamic AutoQueryable<TEntity>(this IQueryable<TEntity> query, string queryString, AutoQueryableProfile profile = null) where TEntity : class
        {
            profile = profile ?? new AutoQueryableProfile();
            Type entityType = typeof(TEntity);
            return QueryableHelper.GetAutoQuery(queryString, entityType, query, profile);
        }

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
    }
}
