using System.Collections.Generic;
using System.Linq.Expressions;
using AutoQueryable.Core.Aliases;

namespace AutoQueryable.Core.CriteriaFilters.FilterMaps
{
    public class DateTimeFilterMapFactory
    {
        public static IQueryableFilterMap QueryableFilterMap;
        public static ICollection<IQueryableTypeFilter> InitializeMap(IQueryableFilterMap queryableFilterMap)
        {
            var list = new List<IQueryableTypeFilter>();
            QueryableFilterMap = queryableFilterMap;
            list.AddRange(BaseFilterMapFactory.InitializeMap(queryableFilterMap));
            list.AddRange(SpecificFilterMap);

            return list;
        }

        private static IEnumerable<IQueryableTypeFilter> SpecificFilterMap => new List<IQueryableTypeFilter>
        {
            new QueryableTypeFilter(QueryableFilterMap.GetFilter(ConditionAlias.DateInYear), (left, right) => Expression.Equal(Expression.PropertyOrField(left, "Year"), Expression.PropertyOrField(right, "Year"))),
            new QueryableTypeFilter(QueryableFilterMap.GetFilter(ConditionAlias.DateNotInYear), (left, right) => Expression.Negate(Expression.Equal(Expression.PropertyOrField(left, "Year"), Expression.PropertyOrField(right, "Year")))),
        };

    }
}