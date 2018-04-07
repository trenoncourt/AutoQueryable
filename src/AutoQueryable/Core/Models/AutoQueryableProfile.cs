using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models.Abstractions;

namespace AutoQueryable.Core.Models
{
    public class AutoQueryableProfile
    {
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

        public int? DefaultToTake { get; set; }

        public int? MaxToSkip { get; set; }
        
        public int? MaxDepth { get; set; }
        
        public string DefaultOrderBy { get; set; }
        
        public string DefaultOrderByDesc { get; set; }

        public bool UseBaseType { get; set; }

        public static AutoQueryableProfile From(IFilterProfile filterProfile)
        {
            return new AutoQueryableProfile
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
                DefaultToTake = filterProfile.DefaultToTake == 0 ? null : (int?)filterProfile.DefaultToTake,
                MaxToSkip = filterProfile.MaxToSkip == 0 ? null : (int?)filterProfile.MaxToSkip,
                MaxDepth = filterProfile.MaxDepth == 0 ? null : (int?)filterProfile.MaxDepth,
                DefaultOrderBy = filterProfile.DefaultOrderBy,
                DefaultOrderByDesc = filterProfile.DefaultOrderByDesc,
                UseBaseType = filterProfile.UseBaseType
            };
        }
    }
}
