using System;
using AutoQueryable.Core.CriteriaFilters.FilterMaps;

namespace AutoQueryable.Core.CriteriaFilters
{
    public class CriteriaFilterManager : ICriteriaFilterManager
    {
        private readonly IQueryableFilterMap _queryableFilterMap = new QueryableFilterMap();
        private readonly IQueryableTypeFilterMap _filterMap = new QueryableTypeFilterMap();

        public CriteriaFilterManager()
        {
            _filterMap.AddFilters(typeof(string), StringFilterMapFactory.InitializeMap(_queryableFilterMap));
            _filterMap.AddFilters(typeof(DateTime), DateTimeFilterMapFactory.InitializeMap(_queryableFilterMap));
            _filterMap.AddFilters(typeof(object), BaseFilterMapFactory.InitializeMap(_queryableFilterMap));
            // FilterMap.AddFilters(typeof(DateTime), DateTimeFilter);
        }

        public IQueryableFilter GetFilter(string alias)
            => _queryableFilterMap.GetFilter(alias);

        public IQueryableFilter FindFilter(string queryParameterKey)
            => _queryableFilterMap.FindFilter(queryParameterKey);

        public IQueryableTypeFilter GetTypeFilter(Type type, string alias) =>
            _filterMap.GetFilter(type, alias);
        public IQueryableTypeFilter GetTypeFilter(Type type, Models.Criteria criteria) =>
            _filterMap.GetFilter(type, criteria.Filter);
        public IQueryableTypeFilter GetTypeFilter(Type type, IQueryableFilter queryableFilter) =>
            _filterMap.GetFilter(type, queryableFilter);
        
    }
}
