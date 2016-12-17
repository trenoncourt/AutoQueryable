using System;
using System.Linq;
using AutoQueryable.Helpers;
using AutoQueryable.Models;

namespace AutoQueryable.Extensions
{
    public static class QueryableExtension
    {
        public static dynamic AutoQueryable(this IQueryable<object> query, string queryString, AutoQueryableProfile profile)
        {
            Type entityType = query.GetType().GenericTypeArguments[0];
            return QueryableHelper.GetAutoQuery(queryString, entityType, query, profile);
        }
    }
}
