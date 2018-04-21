using System;
using System.Collections.Generic;

namespace AutoQueryable.Core.CriteriaFilters
{
    public interface IQueryableTypeFilterMap
    {
        void AddFilter(Type type, IQueryableTypeFilter filter);
        void AddFilters(Type type, ICollection<IQueryableTypeFilter> filters);
        IQueryableTypeFilter GetFilter(Type type, string alias);
        IQueryableTypeFilter GetFilter(Type type, IQueryableFilter queryableFilter);
        IQueryableTypeFilter FindFilter(Type type, string queryParameterKey);
    }
}