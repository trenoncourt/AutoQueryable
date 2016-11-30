using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public static Expression ToBinaryExpression(this ConditionType condition, Expression left, Expression right)
        {
            switch (condition)
            {
                case ConditionType.Equal:
                    return Expression.Equal(left, right);
                case ConditionType.NotEqual:
                    return Expression.NotEqual(left, right);
                case ConditionType.Less:
                    return Expression.LessThan(left, right);
                case ConditionType.LessEqual:
                    return Expression.LessThanOrEqual(left, right);
                case ConditionType.Greater:
                    return Expression.GreaterThan(left, right);
                case ConditionType.GreaterEqual:
                    return Expression.GreaterThanOrEqual(left, right);
                case ConditionType.Contains:
                    MethodInfo methodContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    return Expression.Call(left, methodContains, right);
                case ConditionType.StartsWith:
                    MethodInfo methodStartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                    return Expression.Call(left, methodStartsWith, right);
                case ConditionType.EndsWith:
                    MethodInfo methodEndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                    return Expression.Call(left, methodEndsWith, right);
                default:
                    throw new ArgumentOutOfRangeException(nameof(condition), condition, null);
            }
        }
    }
}