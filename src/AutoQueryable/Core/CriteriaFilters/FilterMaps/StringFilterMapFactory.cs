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
                return Expression.Call(left, method, right);
            }),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotContains), (left, right) =>
            {
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                return Expression.Negate(Expression.Call(left, method, right));
            }),


            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.StartsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                return Expression.Call(left, method, right);
            }),  
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotStartsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                return Expression.Negate(Expression.Call(left, method, right));
            }),


            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.EndsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                return Expression.Call(left, method, right);
            }),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotEndsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                return Expression.Negate(Expression.Call(left, method, right));
            }),


            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.EndsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                return Expression.Call(left, method, right);
            }),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotEndsWith), (left, right) =>
            {
                var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                return Expression.Negate(Expression.Call(left, method, right));
            }),
        };
    }
}