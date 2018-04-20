using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class TopClause : Clause
    {
        public TopClause(AutoQueryableContext context) : base(context)
        {
            this.ClauseType = ClauseType.Top;
        }
    }
}