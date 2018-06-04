using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class OrderByClause : Clause
    {
        public OrderByClause()
        {
            ClauseType = ClauseType.OrderBy;
        }
    }
}