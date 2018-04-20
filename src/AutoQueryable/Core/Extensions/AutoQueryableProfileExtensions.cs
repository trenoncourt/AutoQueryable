using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Extensions
{
    public static class AutoQueryableProfileExtensions
    {
        public static bool IsClauseAllowed(this AutoQueryableProfile profile, ClauseType clauseType)
        {
            var isClauseAllowed = true;
            var isAllowed = profile?.AllowedClauses?.HasFlag(clauseType);
            var isDisallowed = profile?.DisAllowedClauses?.HasFlag(clauseType);

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
        
        public static bool IsConditionAllowed(this AutoQueryableProfile profile, ConditionType conditionType)
        {
            var isConditionAllowed = true;
            var isAllowed = profile?.AllowedConditions?.HasFlag(conditionType);
            var isDisallowed = profile?.DisAllowedConditions?.HasFlag(conditionType);

            if (isAllowed.HasValue && !isAllowed.Value)
            {
                isConditionAllowed = false;
            }

            if (isDisallowed.HasValue && isDisallowed.Value)
            {
                isConditionAllowed = false;
            }
            return isConditionAllowed;
        }
        
        public static bool IsWrapperPartAllowed(this AutoQueryableProfile profile, WrapperPartType wrapperPartType)
        {
            var isWrapperPartAllowed = true;
            var isAllowed = profile?.AllowedWrapperPartType?.HasFlag(wrapperPartType);
            var isDisallowed = profile?.DisAllowedWrapperPartType?.HasFlag(wrapperPartType);

            if (isAllowed.HasValue && !isAllowed.Value)
            {
                isWrapperPartAllowed = false;
            }

            if (isDisallowed.HasValue && isDisallowed.Value)
            {
                isWrapperPartAllowed = false;
            }
            return isWrapperPartAllowed;
        }
    }
}