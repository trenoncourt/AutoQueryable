using System;
using System.Linq.Expressions;

namespace AutoQueryable.Core.CriteriaFilters
{
    public interface IQueryableTypeFilter
    {
        Func<Expression, Expression, Expression> Filter { get; set; }
        IQueryableFilter QueryableFilter { get; set; }
    }
}