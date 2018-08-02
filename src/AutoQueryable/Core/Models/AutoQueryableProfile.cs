using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using Serilog;

namespace AutoQueryable.Core.Models
{
    public class AutoQueryableProfile : IAutoQueryableProfile
    {
        public AutoQueryableProfile(AutoQueryableSettings settings)
        {
            SelectableProperties = settings.SelectableProperties;
            UnselectableProperties = settings.UnselectableProperties;
            SortableProperties = settings.SortableProperties;
            UnSortableProperties = settings.UnSortableProperties;
            GroupableProperties = settings.GroupableProperties;
            UnGroupableProperties = settings.UnGroupableProperties;
            AllowedClauses = settings.AllowedClauses;
            DisAllowedClauses = settings.DisAllowedClauses;
            AllowedConditions = settings.AllowedConditions;
            DisAllowedConditions = settings.DisAllowedConditions;
            AllowedWrapperPartType = settings.AllowedWrapperPartType;
            DisAllowedWrapperPartType = settings.DisAllowedWrapperPartType;
            MaxToTake = settings.MaxToTake;
            MaxToSkip = settings.MaxToSkip;
            MaxDepth = settings.MaxDepth;
            DefaultToTake = settings.DefaultToTake;
            DefaultToSelect = settings.DefaultToSelect;
            DefaultOrderBy = settings.DefaultOrderBy;
            UseBaseType = settings.UseBaseType;
            ToListBeforeSelect = settings.ToListBeforeSelect;
        }
        
        public ILogger Logger { get; }

        public string[] SelectableProperties { get; set; }
        
        public string[] UnselectableProperties { get; set; }

        public string[] SortableProperties { get; set; }
        
        public string[] UnSortableProperties { get; set; }

        public string[] GroupableProperties { get; set; }
        
        public string[] UnGroupableProperties { get; set; }

        public ClauseType? AllowedClauses { get; set; }
        
        public ClauseType? DisAllowedClauses { get; set; }

        public ConditionType? AllowedConditions { get; set; }
        
        public ConditionType? DisAllowedConditions { get; set; }

        public WrapperPartType? AllowedWrapperPartType { get; set; }
        
        public WrapperPartType? DisAllowedWrapperPartType { get; set; }

        public int? MaxToTake { get; set; }

        public int? MaxToSkip { get; set; }
        
        public int? MaxDepth { get; set; }

        public int DefaultToTake { get; set; }

        public string DefaultToSelect { get; set; }
        
        public Dictionary<string, bool> DefaultOrderBy { get; set; }
       
        public bool UseBaseType { get; set; }
        public bool ToListBeforeSelect { get; set; }

        public bool IsClauseAllowed(ClauseType clauseType)
        {
            var isClauseAllowed = true;
            var isAllowed = AllowedClauses?.HasFlag(clauseType);
            var isDisallowed = DisAllowedClauses?.HasFlag(clauseType);

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
        
        public bool IsConditionAllowed(ConditionType conditionType)
        {
            var isConditionAllowed = true;
            var isAllowed = AllowedConditions?.HasFlag(conditionType);
            var isDisallowed = DisAllowedConditions?.HasFlag(conditionType);

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
        
        public bool IsWrapperPartAllowed(WrapperPartType wrapperPartType)
        {
            var isWrapperPartAllowed = true;
            var isAllowed = AllowedWrapperPartType?.HasFlag(wrapperPartType);
            var isDisallowed = DisAllowedWrapperPartType?.HasFlag(wrapperPartType);

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
