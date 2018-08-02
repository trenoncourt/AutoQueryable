using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using Serilog;

namespace AutoQueryable.Core.Models
{
    public interface IAutoQueryableProfile
    {
        ILogger Logger { get; }
        string[] SelectableProperties { get; set; }
        string[] UnselectableProperties { get; set; }
        string[] SortableProperties { get; set; }
        string[] UnSortableProperties { get; set; }
        string[] GroupableProperties { get; set; }
        string[] UnGroupableProperties { get; set; }
        ClauseType? AllowedClauses { get; set; }
        ClauseType? DisAllowedClauses { get; set; }
        ConditionType? AllowedConditions { get; set; }
        ConditionType? DisAllowedConditions { get; set; }
        WrapperPartType? AllowedWrapperPartType { get; set; }
        WrapperPartType? DisAllowedWrapperPartType { get; set; }
        int? MaxDepth { get; set; }
        int? MaxToSkip { get; set; }
        int? MaxToTake { get; set; }
        Dictionary<string, bool> DefaultOrderBy { get; set; }
        int DefaultToTake { get; set; }
        string DefaultToSelect { get; set; }
        bool UseBaseType { get; set; }
        
        bool ToListBeforeSelect { get; set; }
        bool IsClauseAllowed(ClauseType clauseType);
        bool IsConditionAllowed(ConditionType conditionType);
        bool IsWrapperPartAllowed(WrapperPartType wrapperPartType);
    }
}