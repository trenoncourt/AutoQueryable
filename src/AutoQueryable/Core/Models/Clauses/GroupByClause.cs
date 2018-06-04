using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class GroupByClause : Clause
    {
        public GroupByClause()
        {
            ClauseType = ClauseType.GroupBy;
        }
    }
}