using AutoQueryable.Models;
using AutoQueryable.Models.Enums;

namespace AutoQueryable.Extensions
{
    public static class AutoQueryableProfileExtensions
    {
        public static bool IsClauseAllowed(this AutoQueryableProfile profile, ClauseType clauseType)
        {
            bool isClauseAllowed = true;
            bool? isAllowed = profile?.AllowedClauses?.HasFlag(clauseType);
            bool? isDisallowed = profile?.DisAllowedClauses?.HasFlag(clauseType);

            if (isAllowed.HasValue && !isAllowed.Value)
            {
                isClauseAllowed = false;
            }

            if (isDisallowed.HasValue && isDisallowed.Value)
            {
                isClauseAllowed = false;
            }
            return isClauseAllowed;
        }
    }
}