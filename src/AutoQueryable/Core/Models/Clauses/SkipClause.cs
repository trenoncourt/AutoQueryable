using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class SkipClause : Clause
    {
        public SkipClause()
        {
            ClauseType = ClauseType.Skip;
        }
    }
}