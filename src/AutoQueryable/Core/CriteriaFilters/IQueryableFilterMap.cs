using System.Collections.Generic;

namespace AutoQueryable.Core.CriteriaFilters
{
    public interface IQueryableFilterMap
    {
        void AddFilter(IQueryableFilter filter);
        void AddFilters(ICollection<IQueryableFilter> filters);
        IQueryableFilter FindFilter(string queryParameterKey);
        IQueryableFilter GetFilter(string alias);
    }
}