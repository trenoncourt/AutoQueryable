using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class OrderByDescClause : Clause
    {
        public OrderByDescClause(AutoQueryableContext context) : base(context)
        {
            this.ClauseType = ClauseType.OrderByDesc;
        }
    }
}