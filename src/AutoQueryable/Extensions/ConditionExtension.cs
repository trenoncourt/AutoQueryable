using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace AutoQueryable.Extensions
{
    public static class ConditionExtension
    {
        public static string ToSqlCondition(this Condition condition, string key, IEnumerable<DbParameter> parameters)
        {
            switch (condition)
            {
                case Condition.Equal:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " = @" + p.ParameterName)) + ")";
                case Condition.NotEqual:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " <> @" + p.ParameterName)) + ")";
                case Condition.Less:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " < @" + p.ParameterName)) + ")";
                case Condition.LessEqual:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " <= @" + p.ParameterName)) + ")";
                case Condition.Greater:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " > @" + p.ParameterName)) + ")";
                case Condition.GreaterEqual:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " >= @" + p.ParameterName)) + ")";
                case Condition.Contains:
                case Condition.StartsWith:
                case Condition.EndsWith:
                    return "(" + string.Join(" OR ", parameters.Select(p => key + " LIKE @" + p.ParameterName)) + ")";
                case Condition.Between:
                    return key + " IN (" + string.Join(", ", parameters.Select(p => "@" + p.ParameterName)) + ")";
                default:
                    // TODO log
                    return "";
            }
        }
    }
}