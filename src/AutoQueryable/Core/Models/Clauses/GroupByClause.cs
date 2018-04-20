using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class GroupByClause : Clause
    {
        public GroupByClause(AutoQueryableContext context) : base(context)
        {
            this.ClauseType = ClauseType.GroupBy;
        }
    }
}