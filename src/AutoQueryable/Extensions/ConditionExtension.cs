using System;
using System.Linq.Expressions;
using System.Reflection;
using AutoQueryable.Models;

namespace AutoQueryable.Extensions
{
    public static class ConditionExtension
    {
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