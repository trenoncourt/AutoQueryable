using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class OrderByDescClause : Clause
    {
        public OrderByDescClause()
        {
            ClauseType = ClauseType.OrderByDesc;
        }
    }
}