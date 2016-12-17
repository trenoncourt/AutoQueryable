using System;
using System.Linq;
using AutoQueryable.Helpers;
using AutoQueryable.Models;

namespace AutoQueryable.Extensions
{
    public static class QueryableExtension
    {
        public static dynamic AutoQueryable<TEntity>(this IQueryable<TEntity> query, string queryString, AutoQueryableProfile profile = null) where TEntity : class
        {
            Type entityType = typeof(TEntity);
            return QueryableHelper.GetAutoQuery(queryString, entityType, query, profile);
        }
    }
}
