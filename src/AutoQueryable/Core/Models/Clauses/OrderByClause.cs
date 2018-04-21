using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class OrderByClause : Clause
    {
        public OrderByClause(AutoQueryableContext context) : base(context)
        {
            this.ClauseType = ClauseType.OrderBy;
        }
    }
}