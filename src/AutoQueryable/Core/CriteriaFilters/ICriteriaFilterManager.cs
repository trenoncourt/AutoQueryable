using System;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.CriteriaFilters
{
    public interface ICriteriaFilterManager
    {
        IQueryableFilter FindFilter(string queryParameterKey);
        IQueryableFilter GetFilter(string alias);
        IQueryableTypeFilter GetTypeFilter(Type type, Criteria criteria);
        IQueryableTypeFilter GetTypeFilter(Type type, IQueryableFilter queryableFilter);
        IQueryableTypeFilter GetTypeFilter(Type type, string alias);
    }
}