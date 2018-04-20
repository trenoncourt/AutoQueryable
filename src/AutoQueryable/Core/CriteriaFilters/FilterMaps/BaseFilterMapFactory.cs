using System.Collections.Generic;
using System.Linq.Expressions;
using AutoQueryable.Core.Aliases;

namespace AutoQueryable.Core.CriteriaFilters.FilterMaps
{
    public class BaseFilterMapFactory
    {
        public static ICollection<IQueryableTypeFilter> InitializeMap(IQueryableFilterMap queryableFilterMap) => new List<IQueryableTypeFilter>
        {
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.Equal), Expression.Equal),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.NotEqual), Expression.NotEqual),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.LessThan), Expression.LessThan),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.LessThanOrEqual), Expression.LessThanOrEqual),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.GreaterThan), Expression.GreaterThan),
            new QueryableTypeFilter(queryableFilterMap.GetFilter(ConditionAlias.GreaterThanOrEqual), Expression.GreaterThanOrEqual),
        };
    }
}