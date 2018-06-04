using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class LastClause : Clause
    {
        public LastClause()
        {
            ClauseType = ClauseType.Last;
        }
    }
}