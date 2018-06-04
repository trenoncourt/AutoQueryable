using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Clauses
{
    public interface IClauseMapManager
    {
        IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey);
        IClauseQueryFilter GetClauseQueryFilter(string alias);
    }
}