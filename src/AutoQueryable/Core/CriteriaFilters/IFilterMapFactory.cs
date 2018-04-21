using System.Collections.Generic;

namespace AutoQueryable.Core.CriteriaFilters
{
    public interface IReZisFilterMapFactory
    {
        ICollection<IQueryableTypeFilter> InitializeMap();
    }
}