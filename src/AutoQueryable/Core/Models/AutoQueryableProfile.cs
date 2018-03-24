using System;
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

        public int? MaxToSkip { get; set; }
        
        public int? MaxDepth { get; set; }

        public ProviderType ProviderType { get; set; } = ProviderType.Default;

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
                AllowedClauses = filterProfile.AllowedClauses,
                DisAllowedClauses = filterProfile.DisAllowedClauses,
                AllowedConditions = filterProfile.AllowedConditions,
                DisAllowedConditions = filterProfile.DisAllowedConditions,
                AllowedWrapperPartType = filterProfile.AllowedWrapperPartType,
                DisAllowedWrapperPartType = filterProfile.DisAllowedWrapperPartType,
                MaxToTake = filterProfile.MaxToTake,
                MaxToSkip = filterProfile.MaxToSkip,
                MaxDepth = filterProfile.MaxDepth
            };
        }
    }
}
