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
        
        public static bool IsConditionAllowed(this AutoQueryableProfile profile, ConditionType conditionType)
        {
            bool isConditionAllowed = true;
            bool? isAllowed = profile?.AllowedConditions?.HasFlag(conditionType);
            bool? isDisallowed = profile?.DisAllowedConditions?.HasFlag(conditionType);

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
            bool isWrapperPartAllowed = true;
            bool? isAllowed = profile?.AllowedWrapperPartType?.HasFlag(wrapperPartType);
            bool? isDisallowed = profile?.DisAllowedWrapperPartType?.HasFlag(wrapperPartType);

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