using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using Serilog;

namespace AutoQueryable.Core.Models
{
    public interface IAutoQueryableProfile
    {
        ClauseType? AllowedClauses { get; set; }
        ConditionType? AllowedConditions { get; set; }
        WrapperPartType? AllowedWrapperPartType { get; set; }
        Dictionary<string, bool> DefaultOrderBy { get; set; }
        int DefaultToTake { get; set; }
        ClauseType? DisAllowedClauses { get; set; }
        ConditionType? DisAllowedConditions { get; set; }
        WrapperPartType? DisAllowedWrapperPartType { get; set; }
        string[] GroupableProperties { get; set; }
        ILogger Logger { get; }
        int? MaxDepth { get; set; }
        int? MaxToSkip { get; set; }
        int? MaxToTake { get; set; }
        string[] SelectableProperties { get; set; }
        string[] SortableProperties { get; set; }
        string[] UnGroupableProperties { get; set; }
        string[] UnselectableProperties { get; set; }
        string[] UnSortableProperties { get; set; }
        bool UseBaseType { get; set; }
        bool ToListBeforeSelect { get; set; }
        bool IsClauseAllowed(ClauseType clauseType);
        bool IsConditionAllowed(ConditionType conditionType);
        bool IsWrapperPartAllowed(WrapperPartType wrapperPartType);
    }
}