using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class FirstClause : Clause
    {
        public FirstClause(AutoQueryableContext context) : base(context)
        {
            ClauseType = ClauseType.First;
        }
    }
}