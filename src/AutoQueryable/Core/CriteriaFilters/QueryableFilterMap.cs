using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.CriteriaFilters.FilterMaps;
using AutoQueryable.Core.CriteriaFilters.Formatters;

namespace AutoQueryable.Core.CriteriaFilters
{
    public class QueryableFilterMap : IQueryableFilterMap
    {
        public QueryableFilterMap() => QueryableFilters = new List<IQueryableFilter>
        {
            new QueryableFilter(ConditionAlias.Equal, 1),
            new QueryableFilter(ConditionAlias.NotEqual, 2),
            new QueryableFilter(ConditionAlias.LessThan, 1),
            new QueryableFilter(ConditionAlias.GreaterThan, 1),
            new QueryableFilter(ConditionAlias.GreaterThanOrEqual, 2),
            new QueryableFilter(ConditionAlias.LessThanOrEqual, 2),

            new QueryableFilter(ConditionAlias.Contains, 10),
            new QueryableFilter(ConditionAlias.NotContains, 10),
            new QueryableFilter(ConditionAlias.StartsWith, 10),
            new QueryableFilter(ConditionAlias.NotStartsWith, 10),
            new QueryableFilter(ConditionAlias.EndsWith, 10),
            new QueryableFilter(ConditionAlias.NotEndsWith, 10),
                
            new QueryableFilter(ConditionAlias.DateInYear, 10, new DateInYearFormatProvider()),
        };

        private ICollection<IQueryableFilter> QueryableFilters { get; }
        public void AddFilter(IQueryableFilter filter)
        {
            QueryableFilters.Add(filter);
        }

        public void AddFilters(ICollection<IQueryableFilter> filters)
        {
            foreach (var queryableFilter in filters)
            {
                QueryableFilters.Add(queryableFilter);
            }
        }

        public IQueryableFilter GetFilter(string alias) => QueryableFilters.OrderByDescending(f => f.Level).FirstOrDefault(f => f.Alias == alias);
        

        public IQueryableFilter FindFilter(string queryParameterKey) => QueryableFilters.OrderByDescending(f => f.Level).FirstOrDefault(f => queryParameterKey.Contains(f.Alias));
    }
}