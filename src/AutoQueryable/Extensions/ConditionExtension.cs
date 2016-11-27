using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AutoQueryable.Models;

namespace AutoQueryable.Extensions
{
    public static class ConditionExtension
    {
        public static string ToSqlCondition(this ConditionType condition, string key, IEnumerable<DbParameter> parameters)
        {
            switch (condition)
            {
                case ConditionType.Equal:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " = @" + p.ParameterName)) + ")";
                case ConditionType.NotEqual:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " <> @" + p.ParameterName)) + ")";
                case ConditionType.Less:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " < @" + p.ParameterName)) + ")";
                case ConditionType.LessEqual:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " <= @" + p.ParameterName)) + ")";
                case ConditionType.Greater:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " > @" + p.ParameterName)) + ")";
                case ConditionType.GreaterEqual:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " >= @" + p.ParameterName)) + ")";
                case ConditionType.Contains:
                case ConditionType.StartsWith:
                case ConditionType.EndsWith:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " LIKE @" + p.ParameterName)) + ")";
                case ConditionType.Between:
                    return key + " IN (" + string.Join(", ", parameters.Select(p => "@" + p.ParameterName)) + ")";
                default:
                    // TODO log
                    return "";
            }
        }
    }
}