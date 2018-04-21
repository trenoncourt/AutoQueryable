using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class SkipClause : Clause
    {
        public SkipClause(AutoQueryableContext context) : base(context)
        {
            this.ClauseType = ClauseType.Skip;
        }
    }
}