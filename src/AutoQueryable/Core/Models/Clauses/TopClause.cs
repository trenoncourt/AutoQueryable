using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class TopClause : Clause
    {
        public TopClause()
        {
            ClauseType = ClauseType.Top;
        }
    }
}