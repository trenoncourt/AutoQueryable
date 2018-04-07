using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class LastClause : Clause
    {
        public LastClause(AutoQueryableContext context) : base(context)
        {
            ClauseType = ClauseType.Last;
        }
    }
}