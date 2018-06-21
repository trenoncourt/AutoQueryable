using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Clauses
{
    public interface IClauseQueryFilterMap
    {
        IClauseQueryFilter GetClauseQueryFilter(string alias);
        IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey);
        IClauseQueryFilter GetClauseQueryFilter(ClauseType clauseType);
    }
}