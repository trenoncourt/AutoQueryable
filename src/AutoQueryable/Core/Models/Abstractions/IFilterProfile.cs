using System.Collections.Generic;
using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Abstractions
{
    public interface IFilterProfile
    {
        string[] SelectableProperties { get; set; }

        string[] UnselectableProperties { get; set; }

        string[] SortableProperties { get; set; }

        string[] UnSortableProperties { get; set; }

        string[] GroupableProperties { get; set; }

        string[] UnGroupableProperties { get; set; }

        ClauseType AllowedClauses { get; set; }

        ClauseType DisAllowedClauses { get; set; }

        ConditionType AllowedConditions { get; set; }

        ConditionType DisAllowedConditions { get; set; }

        WrapperPartType AllowedWrapperPartType { get; set; }

        WrapperPartType DisAllowedWrapperPartType { get; set; }

        int MaxToTake { get; set; }
        
        int DefaultToTake { get; set; }

        int MaxToSkip { get; set; }

        int MaxDepth { get; set; }

        Dictionary<string, bool> DefaultOrderBy { get; set; }
        
        string DefaultOrderByDesc { get; set; }

        bool UseBaseType { get; set; }
    }
}