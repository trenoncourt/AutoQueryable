using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoQueryable.Core.CriteriaFilters
{
    public class QueryableTypeFilterMap : IQueryableTypeFilterMap
    {
        public Type DefaultFilterType { get; set; } = typeof(object);
        private Dictionary<Type,ICollection<IQueryableTypeFilter>> Filters { get; } = new Dictionary<Type, ICollection<IQueryableTypeFilter>>();
        
        public void AddFilter(Type type, IQueryableTypeFilter filter)
        {
            Filters[type].Add(filter);
        }
        public void AddFilters(Type type, ICollection<IQueryableTypeFilter> filters)
        {
            if (!Filters.ContainsKey(type))
            {
                Filters[type] = new List<IQueryableTypeFilter>();
            }
            foreach (var filter in filters)
            {
                Filters[type].Add(filter);
            }
        }

        public IQueryableTypeFilter GetFilter(Type type, string alias)
        { 
            try{
                return !Filters.ContainsKey(type) 
                ? Filters[DefaultFilterType].OrderByDescending(f => f.QueryableFilter.Level).FirstOrDefault(f => f.QueryableFilter.Alias.Equals(alias)) 
                : Filters[type].OrderByDescending(f => f.QueryableFilter.Level).FirstOrDefault(f => f.QueryableFilter.Alias.Equals(alias));
            }
            catch(NullReferenceException exception)
            {
                throw new NullReferenceException($"One of the QueryableTypeFilters in the QueryableTypeFilterMap<{type}> doesn't have a QueryableFilter defined. Make sure it is also added to the QueryableFilterMap.", exception);
            }
        }
        

        public IQueryableTypeFilter GetFilter(Type type, IQueryableFilter queryableFilter) {
            try{
                return !Filters.ContainsKey(type)
                    ? Filters[DefaultFilterType].FirstOrDefault(f => f.QueryableFilter == queryableFilter)
                    : Filters[type].FirstOrDefault(f => f.QueryableFilter == queryableFilter);
            }
            catch(NullReferenceException exception)
            {
                throw new NullReferenceException($"One of the QueryableTypeFilters in the QueryableTypeFilterMap<{type}> doesn't have a QueryableFilter defined. Make sure it is also added to the QueryableFilterMap.", exception);
            }
        }


        public IQueryableTypeFilter FindFilter(Type type, string queryParameterKey)
        {
                return !Filters.ContainsKey(type)
                    ? Filters[DefaultFilterType].OrderByDescending(f => f.QueryableFilter.Level).FirstOrDefault(f => queryParameterKey.Contains(f.QueryableFilter.Alias))
                    : Filters[type].OrderByDescending(f => f.QueryableFilter.Level).FirstOrDefault(f => queryParameterKey.Contains(f.QueryableFilter.Alias));
        }
    }
}