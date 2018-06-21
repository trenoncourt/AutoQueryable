using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using AutoQueryable.Models;

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

        // TODO: Implement WrapWith
        public static dynamic ToAutoQueryListResult<TEntity>(this IQueryable<TEntity> query, IAutoQueryableContext context) where TEntity : class
        {
            if(!context.ClauseValueManager.WrapWith.Any())
            {
                return query.ToList();
            }
            if(context.ClauseValueManager.First)
            {
                return query.FirstOrDefault();
            }
            var result = query.ToList();

            //if (!context.ClauseValueManager.Page.HasValue || context.ClauseValueManager.Page.Value == 0)
            //{
            //    return result;
            //}

            return new PagedResult<TEntity>
            {
                TotalCount = context.TotalCountQuery?.Count() ?? result.Count,
                RowCount = result.Count,
                Result = result
            };
        }
        //private static dynamic _getWrappedResult(QueryResult queryResult, IAutoQueryableContext context)
        //{
        //    dynamic wrapper = new ExpandoObject();
        //    wrapper.Result = (queryResult.Result as IQueryable<object>).ToList();
        //    foreach (var part in this._wrapperPartTypes)
        //    {
        //        switch (part)
        //        {
        //            case WrapperPartType.Count:
        //                var isResultEnumerableCount = typeof(IEnumerable).IsAssignableFrom((Type)queryResult.Result.GetType());
        //                if (isResultEnumerableCount)
        //                {
        //                    wrapper.Count = wrapper.Result.Count;
        //                }
        //                break;
        //            case WrapperPartType.TotalCount:
        //                wrapper.TotalCount = queryResult.TotalCount;
        //                break;
        //            case WrapperPartType.NextLink:
        //                var isResultEnumerableNextLink = typeof(IEnumerable).IsAssignableFrom((Type)queryResult.Result.GetType());

        //                if (isResultEnumerableNextLink && context.ClauseValueManager.Top != null)
        //                {
        //                    var skip = context.ClauseValueManager.Skip == null ? 0 : Convert.ToInt32(context.ClauseValueManager.Skip.Value);
        //                    var take = Convert.ToInt32(context.ClauseValueManager.Top.Value);
        //                    skip += take;
        //                    if (wrapper.Result.Count < take)
        //                    {
        //                        break;
        //                    }
        //                    if (context.ClauseValueManager.Skip != null)
        //                    {
        //                        wrapper.NextLink = context.QueryString.ToLower().Replace($"{ClauseAlias.Skip}{context.ClauseValueManager.Skip.Value}", $"{ClauseAlias.Skip}{skip}");
        //                    }
        //                    else
        //                    {
        //                        wrapper.NextLink = $"{context.QueryString.ToLower()}&{ClauseAlias.Skip}{skip}";
        //                    }

        //                }
        //                break;
        //        }
        //    }
        //    return wrapper;
        //}
    }

}
