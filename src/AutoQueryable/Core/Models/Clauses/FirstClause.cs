using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class FirstClause : Clause
    {
        public FirstClause()
        {
            ClauseType = ClauseType.First;
        }
    }
}