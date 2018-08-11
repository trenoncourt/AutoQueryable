using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Clauses
{
    public interface IClauseMapManager
    {
        void Init();
        IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey);
        IClauseQueryFilter GetClauseQueryFilter(ClauseType clauseType);
    }
}