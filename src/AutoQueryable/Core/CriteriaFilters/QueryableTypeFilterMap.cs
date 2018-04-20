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
            this.Filters[type].Add(filter);
        }
        public void AddFilters(Type type, ICollection<IQueryableTypeFilter> filters)
        {
            if (!this.Filters.ContainsKey(type))
            {
                this.Filters[type] = new List<IQueryableTypeFilter>();
            }
            foreach (var filter in filters)
            {
                this.Filters[type].Add(filter);
            }
        }

        public IQueryableTypeFilter GetFilter(Type type, string alias) => !this.Filters.ContainsKey(type) 
            ? this.Filters[this.DefaultFilterType].OrderByDescending(f => f.QueryableFilter.Level).FirstOrDefault(f => f.QueryableFilter.Alias.Equals(alias)) 
            : this.Filters[type].OrderByDescending(f => f.QueryableFilter.Level).FirstOrDefault(f => f.QueryableFilter.Alias.Equals(alias));
        

        public IQueryableTypeFilter GetFilter(Type type, IQueryableFilter queryableFilter) =>
            !this.Filters.ContainsKey(type)
                ? this.Filters[this.DefaultFilterType].FirstOrDefault(f => f.QueryableFilter == queryableFilter)
                : this.Filters[type].FirstOrDefault(f => f.QueryableFilter == queryableFilter);

        public IQueryableTypeFilter FindFilter(Type type, string queryParameterKey)
        {
            return !this.Filters.ContainsKey(type)
                ? this.Filters[this.DefaultFilterType].OrderByDescending(f => f.QueryableFilter.Level).FirstOrDefault(f => queryParameterKey.Contains(f.QueryableFilter.Alias))
                : this.Filters[type].OrderByDescending(f => f.QueryableFilter.Level).FirstOrDefault(f => queryParameterKey.Contains(f.QueryableFilter.Alias));
        }
    }
}