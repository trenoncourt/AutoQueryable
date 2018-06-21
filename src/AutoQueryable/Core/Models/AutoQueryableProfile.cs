using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models.Abstractions;
using Serilog;

namespace AutoQueryable.Core.Models
{
    public class AutoQueryableProfile : IAutoQueryableProfile
    {
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

        public int DefaultToTake { get; set; } = 10;

        public int? MaxToSkip { get; set; }
        
        public int? MaxDepth { get; set; }
        
        public Dictionary<string, bool> DefaultOrderBy { get; set; } = new Dictionary<string, bool>();
       

        public bool UseBaseType { get; set; }
        public bool ToListBeforeSelect { get; set; }

        public static AutoQueryableProfile From(IFilterProfile filterProfile) => new AutoQueryableProfile
        {
            SelectableProperties = filterProfile.SelectableProperties,
            UnselectableProperties = filterProfile.UnselectableProperties,
            SortableProperties = filterProfile.SortableProperties,
            UnSortableProperties = filterProfile.UnSortableProperties,
            GroupableProperties = filterProfile.GroupableProperties,
            UnGroupableProperties = filterProfile.UnGroupableProperties,
            AllowedClauses = filterProfile.AllowedClauses == ClauseType.None ? null : (ClauseType?)filterProfile.AllowedClauses,
            DisAllowedClauses = filterProfile.DisAllowedClauses == ClauseType.None ? null : (ClauseType?)filterProfile.DisAllowedClauses,
            AllowedConditions = filterProfile.AllowedConditions == ConditionType.None ? null : (ConditionType?)filterProfile.AllowedConditions,
            DisAllowedConditions = filterProfile.DisAllowedConditions == ConditionType.None ? null : (ConditionType?)filterProfile.DisAllowedConditions,
            AllowedWrapperPartType = filterProfile.AllowedWrapperPartType == WrapperPartType.None ? null : (WrapperPartType?)filterProfile.AllowedWrapperPartType,
            DisAllowedWrapperPartType = filterProfile.DisAllowedWrapperPartType == WrapperPartType.None ? null : (WrapperPartType?)filterProfile.DisAllowedWrapperPartType,
            MaxToTake = filterProfile.MaxToTake == 0 ? null : (int?)filterProfile.MaxToTake,
            DefaultToTake = filterProfile.DefaultToTake,
            MaxToSkip = filterProfile.MaxToSkip == 0 ? null : (int?)filterProfile.MaxToSkip,
            MaxDepth = filterProfile.MaxDepth == 0 ? null : (int?)filterProfile.MaxDepth,
            DefaultOrderBy = filterProfile.DefaultOrderBy,
            UseBaseType = filterProfile.UseBaseType,
            ToListBeforeSelect = filterProfile.ToListBeforeSelect
        };
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
