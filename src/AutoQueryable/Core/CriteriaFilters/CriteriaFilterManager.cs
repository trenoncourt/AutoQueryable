using System;
using AutoQueryable.Core.CriteriaFilters.FilterMaps;

namespace AutoQueryable.Core.CriteriaFilters
{
    public static class CriteriaFilterManager
    {
        private static readonly IQueryableFilterMap QueryableFilterMap = new QueryableFilterMap();
        private static readonly IQueryableTypeFilterMap FilterMap = new QueryableTypeFilterMap();

        public static void InitializeFilterMap()
        {
            FilterMap.AddFilters(typeof(string), StringFilterMapFactory.InitializeMap(QueryableFilterMap));
            FilterMap.AddFilters(typeof(DateTime), DateTimeFilterMapFactory.InitializeMap(QueryableFilterMap));
            FilterMap.AddFilters(typeof(object), BaseFilterMapFactory.InitializeMap(QueryableFilterMap));
            // FilterMap.AddFilters(typeof(DateTime), DateTimeFilter);
        }


        public static IQueryableFilter GetFilter(string alias)
            => QueryableFilterMap.GetFilter(alias);

        public static IQueryableFilter FindFilter(string queryParameterKey)
            => QueryableFilterMap.FindFilter(queryParameterKey);

        public static IQueryableTypeFilter GetTypeFilter(Type type, string alias) =>
            FilterMap.GetFilter(type, alias);
        public static IQueryableTypeFilter GetTypeFilter(Type type, Models.Criteria criteria) =>
            FilterMap.GetFilter(type, criteria.Filter);
        public static IQueryableTypeFilter GetTypeFilter(Type type, IQueryableFilter queryableFilter) =>
            FilterMap.GetFilter(type, queryableFilter);
        
    }
}
