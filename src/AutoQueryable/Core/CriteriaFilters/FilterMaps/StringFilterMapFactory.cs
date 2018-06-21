using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AutoQueryable.Core.Aliases;

namespace AutoQueryable.Core.CriteriaFilters.FilterMaps
{
    public class StringFilterMapFactory
    {
        public static ICollection<IQueryableTypeFilter> InitializeMap(IQueryableFilterMap queryableFilterMap)
        {
            var list = new List<IQueryableTypeFilter>();

            list.AddRange(BaseFilterMapFactory.InitializeMap(queryableFilterMap));
            list.AddRange(SpecificFilterMap(queryableFilterMap));

            return list;
        }
        private static IEnumerable<IQueryableTypeFilter> SpecificFilterMap(IQueryableFilterMap queryableFilterMap) => new List<IQueryableTypeFilter>
        {
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.Contains), (left, right) =>
            {
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                return _addNotNull(left, Expression.Call(left, method, right));
            }),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotContains), (left, right) =>
            {
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                return _addNull(left, Expression.Not(Expression.Call(left, method, right)));
            }),


            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.StartsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                return _addNotNull(left, Expression.Call(left, method, right));
            }),  
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotStartsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                return _addNull(left, Expression.Not(Expression.Call(left, method, right)));
            }),


            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.EndsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                return _addNotNull(left, Expression.Call(left, method, right));
            }),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotEndsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                return _addNull(left, Expression.Not(Expression.Call(left, method, right)));
            }),


            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.ContainsIgnoreCase), (left, right) =>
            {
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                var leftToLower = Expression.Call(left, "ToLowerInvariant", null);
                var rigthToLower = Expression.Call(right, "ToLowerInvariant", null);

                return _addNotNull(left, Expression.Call(leftToLower, method, rigthToLower));
            }),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotContainsIgnoreCase), (left, right) =>
            {
                var method = typeof(string).GetMethod("IndexOf", new[] { typeof(string) });
                
                var leftToLower = Expression.Call(left, "ToLowerInvariant", null);
                var rigthToLower = Expression.Call(right, "ToLowerInvariant", null);

                return _addNull(left, Expression.Not(Expression.Call(leftToLower, method, rigthToLower)));
            }),


            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.StartsWithIgnoreCase), (left, right) =>
            {
                var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });

                var leftToLower = Expression.Call(left, "ToLowerInvariant", null);
                var rigthToLower = Expression.Call(right, "ToLowerInvariant", null);

                return _addNotNull(left, Expression.Call(leftToLower, method, rigthToLower));
            }),  
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotStartsWithIgnoreCase), (left, right) =>
            {
                var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });

                var leftToLower = Expression.Call(left, "ToLowerInvariant", null);
                var rigthToLower = Expression.Call(right, "ToLowerInvariant", null);

                return _addNull(left, Expression.Not(Expression.Call(leftToLower, method, rigthToLower)));
            }),


            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.EndsWithIgnoreCase), (left, right) =>
            {
                var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                //Expression.s

                var leftToLower = Expression.Call(left, "ToLowerInvariant", null);
                var rigthToLower = Expression.Call(right, "ToLowerInvariant", null);

                return _addNotNull(left, Expression.Call(leftToLower, method, rigthToLower));
            }),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotEndsWithIgnoreCase), (left, right) =>
            {
                var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

                var leftToLower = Expression.Call(left, "ToLowerInvariant", null);
                var rigthToLower = Expression.Call(right, "ToLowerInvariant", null);

                return _addNull(left, Expression.Not(Expression.Call(leftToLower, method, rigthToLower)));
            }),
        };

        private static Expression _addNotNull(Expression onExpression, Expression methodCall) => Expression.AndAlso(Expression.NotEqual(onExpression, Expression.Constant(null, typeof(object))), methodCall);
        private static Expression _addNull(Expression onExpression, Expression methodCall) => Expression.OrElse(Expression.Equal(onExpression, Expression.Constant(null, typeof(object))), methodCall);
    }
}